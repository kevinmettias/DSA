using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Course Schedule IV (LC 1462): a fresh BFS reachability check per query
// (baseline - O(V+E) every time, the textbook per-query approach with no
// precomputation) vs. a single
// AllPairsShortestPaths.TryComputeDistances call (Floyd-Warshall) that answers
// every (u, v) pair up front, after which each query is an O(1) dictionary
// lookup - the same "one all-pairs matrix, many pair lookups" shape
// FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks.cs
// already established for this primitive. Prerequisite edges only ever point
// from a lower to a higher course id, which keeps the generated graph acyclic
// (a real prerequisite DAG) without needing a separate cycle check.
[MemoryDiagnoser]
public class CourseScheduleIVBenchmarks
{
    private const int QueryCount = 300;

    // LC problem number, used as the deterministic Random seed.
    private const int RandomSeed = 1462;

    private const int EdgesPerCourse = 2;

    [Params(50, 150)]
    public int CourseCount;

    private List<WeightedGraphNode> _courses = null!;
    private (int From, int To)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _courses = [.. Enumerable.Range(0, CourseCount).Select(id => new WeightedGraphNode(id))];

        for (var i = 0; i < CourseCount - 1; i++)
        {
            for (var e = 0; e < EdgesPerCourse; e++)
            {
                var to = i + 1 + random.Next(CourseCount - i - 1);
                _courses[i].Edges.Add((1, _courses[to]));
            }
        }

        _queries = new (int, int)[QueryCount];
        for (var q = 0; q < QueryCount; q++)
        {
            var from = random.Next(CourseCount);
            var to = random.Next(CourseCount);
            _queries[q] = (from, to);
        }
    }

    [Benchmark(Baseline = true)]
    public int BfsPerQuery()
    {
        var matches = 0;

        foreach (var (from, to) in _queries)
        {
            if (IsReachable(from, to))
            {
                matches++;
            }
        }

        return matches;
    }

    [Benchmark]
    public int FloydWarshallAllPairs()
    {
        AllPairsShortestPaths.TryComputeDistances<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(
            _courses, out var distances);

        var matches = 0;

        foreach (var (from, to) in _queries)
        {
            if (distances.ContainsKey((_courses[from], _courses[to])))
            {
                matches++;
            }
        }

        return matches;
    }

    private bool IsReachable(int from, int to)
    {
        if (from == to)
        {
            return true;
        }

        var (visited, pending) = CreateBfsFrontier(from);
        return BfsReachesTarget(visited, pending, to);
    }

    private (bool[] Visited, Queue<int> Pending) CreateBfsFrontier(int from)
    {
        var visited = new bool[CourseCount];
        var pending = new Queue<int>();
        visited[from] = true;
        pending.Enqueue(from);

        return (visited, pending);
    }

    private bool BfsReachesTarget(bool[] visited, Queue<int> pending, int to)
    {
        while (pending.Count > 0)
        {
            var current = pending.Dequeue();

            foreach (var (_, target) in _courses[current].Edges)
            {
                if (target.Id == to)
                {
                    return true;
                }

                if (!visited[target.Id])
                {
                    visited[target.Id] = true;
                    pending.Enqueue(target.Id);
                }
            }
        }

        return false;
    }
}
