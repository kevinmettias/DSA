using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Subtrees With Max Distance Between Cities (LC 1617): for every one of the
// 2^n city subsets, computing that subtree's diameter by BFS-ing from every one of
// its cities and taking the max distance seen (baseline, O(V) BFS calls per mask)
// vs. the classic double-BFS trick - one BFS to find a farthest city, a second BFS
// from there whose own farthest distance already is the diameter (O(1) BFS calls
// per mask). Both share this repo's own Queue<int> as the BFS frontier
// (MinimumHeightTreesTests/ShortestPathInBinaryMatrixTests precedent); connectivity
// falls out of the same BFS pass in both, since a mask that leaves any of its own
// cities unreached is not a valid subtree.
[MemoryDiagnoser]
public class CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks
{
    private const int RandomSeed = 1617; // LC problem number
    private const int MinSubtreeCityCount = 2;

    [Params(10, 14)]
    public int N;

    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _edges = GenerateRandomTreeEdges(N, random);
    }

    [Benchmark(Baseline = true)]
    public int[] AllPairsBfsPerMask() => CountSubtreesWithMaxDistance(N, _edges, TryComputeDiameterAllPairs);

    [Benchmark]
    public int[] DoubleBfsPerMask() => CountSubtreesWithMaxDistance(N, _edges, TryComputeDiameterDoubleBfs);

    private delegate bool DiameterStrategy(int mask, int n, List<int>[] adjacency, out int diameter);

    private static int[] CountSubtreesWithMaxDistance(int n, int[][] edges, DiameterStrategy computeDiameter)
    {
        var adjacency = BuildAdjacency(n, edges);
        var counts = new int[n - 1];

        for (var mask = 1; mask < (1 << n); mask++)
        {
            if (PopCount(mask) < MinSubtreeCityCount)
            {
                continue;
            }

            if (computeDiameter(mask, n, adjacency, out var diameter))
            {
                counts[diameter - 1]++;
            }
        }

        return counts;
    }

    private static bool TryComputeDiameterAllPairs(int mask, int n, List<int>[] adjacency, out int diameter)
    {
        var accumulation = new DiameterAccumulation { Diameter = 0, Connected = true };

        for (var start = 0; start < n; start++)
        {
            if ((mask & (1 << start)) == 0)
            {
                continue;
            }

            var distances = BfsDistances(start, mask, n, adjacency);
            AccumulateDiameter(mask, n, distances, ref accumulation);
        }

        diameter = accumulation.Diameter;
        return accumulation.Connected;
    }

    private static void AccumulateDiameter(int mask, int n, int[] distances, ref DiameterAccumulation accumulation)
    {
        for (var node = 0; node < n; node++)
        {
            if ((mask & (1 << node)) == 0)
            {
                continue;
            }

            if (distances[node] == -1)
            {
                accumulation.Connected = false;
            }
            else if (distances[node] > accumulation.Diameter)
            {
                accumulation.Diameter = distances[node];
            }
        }
    }

    private struct DiameterAccumulation
    {
        public int Diameter;
        public bool Connected;
    }

    private static bool TryComputeDiameterDoubleBfs(int mask, int n, List<int>[] adjacency, out int diameter)
    {
        var start = LowestSetBitIndex(mask);
        var firstPass = BfsDistances(start, mask, n, adjacency);

        if (!TryFindFarthestNode(mask, firstPass, start, out var farthest))
        {
            diameter = 0;
            return false;
        }

        var secondPass = BfsDistances(farthest, mask, n, adjacency);
        diameter = ComputeMaxDistance(mask, n, secondPass);
        return true;
    }

    private static bool TryFindFarthestNode(int mask, int[] distances, int start, out int farthest)
    {
        var search = new FarthestNodeSearch { Farthest = start, MaxDistance = 0 };

        for (var node = 0; node < distances.Length; node++)
        {
            if ((mask & (1 << node)) == 0)
            {
                continue;
            }

            if (distances[node] == -1)
            {
                farthest = start;
                return false;
            }

            UpdateFarthest(node, distances[node], ref search);
        }

        farthest = search.Farthest;
        return true;
    }

    private static void UpdateFarthest(int node, int distance, ref FarthestNodeSearch search)
    {
        if (distance > search.MaxDistance)
        {
            search.MaxDistance = distance;
            search.Farthest = node;
        }
    }

    private struct FarthestNodeSearch
    {
        public int Farthest;
        public int MaxDistance;
    }

    private static int ComputeMaxDistance(int mask, int n, int[] distances)
    {
        var maxDistance = 0;

        for (var node = 0; node < n; node++)
        {
            if ((mask & (1 << node)) != 0 && distances[node] > maxDistance)
            {
                maxDistance = distances[node];
            }
        }

        return maxDistance;
    }

    private static int[] BfsDistances(int start, int mask, int n, List<int>[] adjacency)
    {
        var distance = new int[n];
        Array.Fill(distance, -1);
        distance[start] = 0;

        var frontier = new RepoQueue();
        frontier.Enqueue(start);

        while (frontier.TryDequeue(out var node))
        {
            foreach (var neighbor in adjacency[node])
            {
                if ((mask & (1 << neighbor)) == 0 || distance[neighbor] != -1)
                {
                    continue;
                }

                distance[neighbor] = distance[node] + 1;
                frontier.Enqueue(neighbor);
            }
        }

        return distance;
    }

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            var a = edge[0] - 1;
            var b = edge[1] - 1;
            adjacency[a].Add(b);
            adjacency[b].Add(a);
        }

        return adjacency;
    }

    private static int PopCount(int mask)
    {
        var count = 0;

        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }

    private static int LowestSetBitIndex(int mask)
    {
        var index = 0;

        while ((mask & (1 << index)) == 0)
        {
            index++;
        }

        return index;
    }

    // A random parent-below-index tree, 1-indexed to match the problem's own city
    // numbering (edges reference cities 1..n directly, not 0-indexed).
    private static int[][] GenerateRandomTreeEdges(int n, Random random)
    {
        var edges = new int[n - 1][];

        for (var i = 1; i < n; i++)
        {
            var parent = random.Next(i);
            edges[i - 1] = [parent + 1, i + 1];
        }

        return edges;
    }
}
