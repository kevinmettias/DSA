using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Score of a Path Between Two Cities (LC 2492): a BFS flood-fill from city 1
// over an adjacency list (baseline - the textbook approach, an explicit queue plus a
// visited array) vs. this repo's own DisjointSet unioning every road, then taking the
// minimum weight among roads whose endpoint root matches city 1's root - the same
// Union-Find-for-component-membership move NumberOfProvincesBenchmarks/
// GraphConnectivityWithThresholdBenchmarks already use, applied here to a min-weight
// reduction over one component's edges instead of component counting or pure
// connectivity queries.
[MemoryDiagnoser]
public class MinimumScoreOfAPathBetweenTwoCitiesBenchmarks
{
    private const int RandomSeed = 2492; // LC problem number
    private const int MaxWeightExclusive = 1_000;

    [Params(200, 5_000)]
    public int CityCount;

    private int[][] _roads = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var roads = new List<int[]>();

        // A connected spine 1..CityCount guarantees city 1 and city CityCount share a
        // component, matching this problem's own guarantee, plus extra random chords
        // inside that same spine so more than one candidate minimum weight exists.
        for (var city = 2; city <= CityCount; city++)
        {
            roads.Add([city - 1, city, random.Next(1, MaxWeightExclusive)]);
        }

        for (var extra = 0; extra < CityCount; extra++)
        {
            var a = random.Next(1, CityCount + 1);
            var b = random.Next(1, CityCount + 1);

            if (a != b)
            {
                roads.Add([a, b, random.Next(1, MaxWeightExclusive)]);
            }
        }

        _roads = roads.ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BreadthFirstFloodFill()
    {
        var adjacency = new List<(int Neighbor, int Weight)>[CityCount + 1];
        for (var city = 1; city <= CityCount; city++)
        {
            adjacency[city] = [];
        }

        foreach (var road in _roads)
        {
            adjacency[road[0]].Add((road[1], road[2]));
            adjacency[road[1]].Add((road[0], road[2]));
        }

        var visited = new bool[CityCount + 1];
        var queue = new Queue<int>();
        queue.Enqueue(1);
        visited[1] = true;
        var minScore = int.MaxValue;

        while (queue.Count > 0)
        {
            var city = queue.Dequeue();

            foreach (var (neighbor, weight) in adjacency[city])
            {
                minScore = Math.Min(minScore, weight);

                if (!visited[neighbor])
                {
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return minScore;
    }

    [Benchmark]
    public int DisjointSetUnionFind()
    {
        var components = new DisjointSet(CityCount + 1);

        foreach (var road in _roads)
        {
            components.Union(road[0], road[1]);
        }

        var targetRoot = components.Find(1);
        var minScore = int.MaxValue;

        foreach (var road in _roads)
        {
            if (components.Find(road[0]) == targetRoot)
            {
                minScore = Math.Min(minScore, road[2]);
            }
        }

        return minScore;
    }
}
