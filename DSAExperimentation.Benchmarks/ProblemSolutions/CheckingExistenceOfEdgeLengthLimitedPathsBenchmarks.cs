using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Checking Existence of Edge Length Limited Paths (LC 1697): a per-query DFS
// baseline that walks the raw edge list from scratch for every query
// (skipping any edge whose weight isn't strictly below that query's limit)
// vs. this repo's own DisjointSet running the standard offline sweep - sort
// edges by weight and queries by limit, both ascending, then union every
// edge below the current limit before answering with one IsConnected lookup,
// the same DisjointSet primitive FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks/
// CouplesHoldingHandsBenchmarks already compose. O(q*(n+e)) baseline vs.
// O((e+q) log(e+q) + (e+q)*alpha(n)) offline.
[MemoryDiagnoser]
public class CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks
{
    private const int RandomSeed = 1697; // LC problem number
    private const int EdgeCountPerNodeMultiplier = 3;
    private const int QueryCountPerNodeMultiplier = 2;
    private const int MaxEdgeWeight = 1_000_000;
    private const int EdgeWeightIndex = 2;
    private const int QueryLimitIndex = 2;

    [Params(100, 2_000)]
    public int NodeCount;

    private int[][] _edgeList = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edgeCount = NodeCount * EdgeCountPerNodeMultiplier;
        var queryCount = NodeCount * QueryCountPerNodeMultiplier;

        _edgeList = Enumerable.Range(0, edgeCount)
            .Select(_ => new[] { random.Next(NodeCount), random.Next(NodeCount), random.Next(1, MaxEdgeWeight) })
            .ToArray();

        _queries = Enumerable.Range(0, queryCount)
            .Select(_ => new[] { random.Next(NodeCount), random.Next(NodeCount), random.Next(1, MaxEdgeWeight) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool[] DfsPerQuery()
    {
        var adjacency = new List<(int To, int Weight)>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in _edgeList)
        {
            adjacency[edge[0]].Add((edge[1], edge[EdgeWeightIndex]));
            adjacency[edge[1]].Add((edge[0], edge[EdgeWeightIndex]));
        }

        var results = new bool[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            results[i] = HasLimitedPath(adjacency, _queries[i][0], _queries[i][1], _queries[i][QueryLimitIndex]);
        }

        return results;
    }

    private bool HasLimitedPath(List<(int To, int Weight)>[] adjacency, int start, int target, int limit)
    {
        if (start == target)
        {
            return true;
        }

        var query = new PathQuery(target, limit);
        return TraverseForTarget(adjacency, start, query);
    }

    private bool TraverseForTarget(List<(int To, int Weight)>[] adjacency, int start, PathQuery query)
    {
        var visited = new bool[NodeCount];
        var stack = new Stack<int>();
        stack.Push(start);
        visited[start] = true;

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (VisitNeighbors(adjacency[node], query, visited, stack))
            {
                return true;
            }
        }

        return false;
    }

    private static bool VisitNeighbors(List<(int To, int Weight)> neighbors, PathQuery query, bool[] visited, Stack<int> stack)
    {
        foreach (var (to, weight) in neighbors)
        {
            if (weight >= query.Limit || visited[to])
            {
                continue;
            }

            if (to == query.Target)
            {
                return true;
            }

            visited[to] = true;
            stack.Push(to);
        }

        return false;
    }

    [Benchmark]
    public bool[] OfflineDisjointSetSweep()
    {
        var edgesByWeight = _edgeList.OrderBy(edge => edge[EdgeWeightIndex]).ToArray();
        var queryOrder = Enumerable.Range(0, _queries.Length).OrderBy(i => _queries[i][QueryLimitIndex]).ToArray();

        var components = new DisjointSet(NodeCount);
        var results = new bool[_queries.Length];
        var edgeIndex = 0;

        foreach (var queryIndex in queryOrder)
        {
            var query = _queries[queryIndex];

            while (edgeIndex < edgesByWeight.Length && edgesByWeight[edgeIndex][EdgeWeightIndex] < query[QueryLimitIndex])
            {
                components.Union(edgesByWeight[edgeIndex][0], edgesByWeight[edgeIndex][1]);
                edgeIndex++;
            }

            results[queryIndex] = components.IsConnected(query[0], query[1]);
        }

        return results;
    }

    private readonly record struct PathQuery(int Target, int Limit);
}
