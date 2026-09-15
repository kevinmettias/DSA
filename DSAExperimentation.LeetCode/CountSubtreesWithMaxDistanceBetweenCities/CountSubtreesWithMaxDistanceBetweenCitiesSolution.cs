using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.CountSubtreesWithMaxDistanceBetweenCities;

// LeetCode 1617. Count Subtrees With Max Distance Between Cities: for d = 1..n-1,
// how many subsets of the cities induce a connected subtree whose two farthest
// cities are exactly d apart.
//
// n <= 15, so every city subset is a bitmask and both strategies try all of them.
// A mask is a valid subtree iff a BFS confined to the mask reaches every city in
// it; the two strategies differ only in how many BFS passes they spend measuring
// that mask's diameter - one per member city (baseline) versus the classic two
// (a BFS to find some farthest city, then a BFS from there whose own farthest
// distance already is the diameter, valid because any connected subset of a tree
// is itself a tree).
internal static class CountSubtreesWithMaxDistanceBetweenCitiesSolution
{
    // A single city is a subtree with no distance to report, so masks below this
    // size never contribute to any bucket.
    private const int MinSubtreeCityCount = 2;

    // Distance-array marker for a city the confined BFS never reached.
    private const int Unreached = -1;

    // Both strategies are stateless, so one instance each serves every call and the
    // benchmark arms that measure these two methods allocate nothing to pick one.
    private static readonly IDiameterStrategy AllPairsBfsDiameter = new DiameterByAllPairsBfs();
    private static readonly IDiameterStrategy DoubleBfsDiameter = new DiameterByDoubleBfs();

    // The textbook baseline: for each city in the mask, BFS from it and take the
    // largest distance seen anywhere in the mask, which is the mask's diameter once
    // every start has been tried. Connectivity falls out of the same passes - a mask
    // that leaves one of its own cities unreached is not a subtree. Deliberately
    // written with BCL collections only; it is the arm the double-BFS strategy below
    // has to justify itself against.
    public static int[] CountSubtreesByAllPairsBfs(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);

        return CountSubtreesByAllPairsBfs(adjacency);
    }

    public static int[] CountSubtreesByAllPairsBfs(List<int>[] adjacency) =>
        CountSubtrees(adjacency, AllPairsBfsDiameter);

    // The same mask enumeration, but each mask's diameter costs a constant two BFS
    // passes instead of one per member city. The frontier is this repo's own
    // Queue<int>, the same adjacency-list-plus-Queue shape MinimumHeightTreesSolution
    // uses for its own edge-list search.
    public static int[] CountSubtreesByDoubleBfs(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);

        return CountSubtreesByDoubleBfs(adjacency);
    }

    public static int[] CountSubtreesByDoubleBfs(List<int>[] adjacency) =>
        CountSubtrees(adjacency, DoubleBfsDiameter);

    // The one question the two arms answer differently: this mask's diameter, and
    // whether the mask holds together at all. The operation and both of its inputs are
    // named here, and what `false` means - a city inside the mask the confined BFS
    // never reached, so the mask is not a subtree - has somewhere to be stated.
    private interface IDiameterStrategy
    {
        bool TryCompute(int mask, List<int>[] adjacency, out int diameter);
    }

    private static int[] CountSubtrees(List<int>[] adjacency, IDiameterStrategy computeDiameter)
    {
        var n = adjacency.Length;
        var counts = new int[n - 1];

        for (var mask = 1; mask < (1 << n); mask++)
        {
            if (PopCount(mask) < MinSubtreeCityCount)
            {
                continue;
            }

            if (computeDiameter.TryCompute(mask, adjacency, out var diameter))
            {
                counts[diameter - 1]++;
            }
        }

        return counts;
    }

    private sealed class DiameterByAllPairsBfs : IDiameterStrategy
    {
        public bool TryCompute(int mask, List<int>[] adjacency, out int diameter)
        {
            var accumulation = new DiameterAccumulation { Diameter = 0, Connected = true };

            for (var start = 0; start < adjacency.Length; start++)
            {
                if ((mask & (1 << start)) == 0)
                {
                    continue;
                }

                var distances = BclBfsDistances(start, mask, adjacency);
                AccumulateDiameter(mask, distances, accumulation);
            }

            diameter = accumulation.Diameter;
            return accumulation.Connected;
        }
    }

    private static void AccumulateDiameter(int mask, int[] distances, DiameterAccumulation accumulation)
    {
        for (var node = 0; node < distances.Length; node++)
        {
            if ((mask & (1 << node)) == 0)
            {
                continue;
            }

            if (distances[node] == Unreached)
            {
                accumulation.Connected = false;
            }
            else if (distances[node] > accumulation.Diameter)
            {
                accumulation.Diameter = distances[node];
            }
        }
    }

    private sealed class DiameterAccumulation
    {
        public int Diameter { get; set; }
        public bool Connected { get; set; }
    }

    private sealed class DiameterByDoubleBfs : IDiameterStrategy
    {
        public bool TryCompute(int mask, List<int>[] adjacency, out int diameter)
        {
            var start = LowestSetBitIndex(mask);
            var firstPass = QueueBfsDistances(start, mask, adjacency);

            if (!TryFindFarthestNode(mask, firstPass, start, out var farthest))
            {
                diameter = 0;
                return false;
            }

            var secondPass = QueueBfsDistances(farthest, mask, adjacency);
            diameter = MaxDistanceInMask(mask, secondPass);
            return true;
        }
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

            if (distances[node] == Unreached)
            {
                farthest = start;
                return false;
            }

            UpdateFarthest(node, distances[node], search);
        }

        farthest = search.Farthest;
        return true;
    }

    private static void UpdateFarthest(int node, int distance, FarthestNodeSearch search)
    {
        if (distance > search.MaxDistance)
        {
            search.MaxDistance = distance;
            search.Farthest = node;
        }
    }

    private sealed class FarthestNodeSearch
    {
        public int Farthest { get; set; }
        public int MaxDistance { get; set; }
    }

    private static int MaxDistanceInMask(int mask, int[] distances)
    {
        var maxDistance = 0;

        for (var node = 0; node < distances.Length; node++)
        {
            if ((mask & (1 << node)) != 0 && distances[node] > maxDistance)
            {
                maxDistance = distances[node];
            }
        }

        return maxDistance;
    }

    // Baseline frontier: a BCL Queue, so the arm stays "what you would write without
    // this repo" (ARCHITECTURE.md 17.5).
    private static int[] BclBfsDistances(int start, int mask, List<int>[] adjacency)
    {
        var distance = NewDistanceArray(adjacency.Length, start);
        var frontier = new Queue<int>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            var node = frontier.Dequeue();

            foreach (var neighbor in adjacency[node])
            {
                if ((mask & (1 << neighbor)) == 0 || distance[neighbor] != Unreached)
                {
                    continue;
                }

                distance[neighbor] = distance[node] + 1;
                frontier.Enqueue(neighbor);
            }
        }

        return distance;
    }

    private static int[] QueueBfsDistances(int start, int mask, List<int>[] adjacency)
    {
        var distance = NewDistanceArray(adjacency.Length, start);
        var frontier = new RepoQueue();
        frontier.Enqueue(start);

        while (frontier.TryDequeue(out var node))
        {
            foreach (var neighbor in adjacency[node])
            {
                if ((mask & (1 << neighbor)) == 0 || distance[neighbor] != Unreached)
                {
                    continue;
                }

                distance[neighbor] = distance[node] + 1;
                frontier.Enqueue(neighbor);
            }
        }

        return distance;
    }

    private static int[] NewDistanceArray(int n, int start)
    {
        var distance = new int[n];
        Array.Fill(distance, Unreached);
        distance[start] = 0;

        return distance;
    }

    // LeetCode numbers the cities 1..n and the edge list references them directly,
    // so each endpoint drops by one to index the adjacency array.
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
}
