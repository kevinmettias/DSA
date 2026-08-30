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
    [Params(100, 2_000)]
    public int NodeCount;

    private int[][] _edgeList = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1697);
        var edgeCount = NodeCount * 3;
        var queryCount = NodeCount * 2;

        _edgeList = Enumerable.Range(0, edgeCount)
            .Select(_ => new[] { random.Next(NodeCount), random.Next(NodeCount), random.Next(1, 1_000_000) })
            .ToArray();

        _queries = Enumerable.Range(0, queryCount)
            .Select(_ => new[] { random.Next(NodeCount), random.Next(NodeCount), random.Next(1, 1_000_000) })
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
            adjacency[edge[0]].Add((edge[1], edge[2]));
            adjacency[edge[1]].Add((edge[0], edge[2]));
        }

        var results = new bool[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            results[i] = HasLimitedPath(adjacency, _queries[i][0], _queries[i][1], _queries[i][2]);
        }

        return results;
    }

    private bool HasLimitedPath(List<(int To, int Weight)>[] adjacency, int start, int target, int limit)
    {
        if (start == target)
        {
            return true;
        }

        var visited = new bool[NodeCount];
        var stack = new Stack<int>();
        stack.Push(start);
        visited[start] = true;

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            foreach (var (to, weight) in adjacency[node])
            {
                if (weight >= limit || visited[to])
                {
                    continue;
                }

                if (to == target)
                {
                    return true;
                }

                visited[to] = true;
                stack.Push(to);
            }
        }

        return false;
    }

    [Benchmark]
    public bool[] OfflineDisjointSetSweep()
    {
        var edgesByWeight = _edgeList.OrderBy(edge => edge[2]).ToArray();
        var queryOrder = Enumerable.Range(0, _queries.Length).OrderBy(i => _queries[i][2]).ToArray();

        var components = new DisjointSet(NodeCount);
        var results = new bool[_queries.Length];
        var edgeIndex = 0;

        foreach (var queryIndex in queryOrder)
        {
            var query = _queries[queryIndex];

            while (edgeIndex < edgesByWeight.Length && edgesByWeight[edgeIndex][2] < query[2])
            {
                components.Union(edgesByWeight[edgeIndex][0], edgesByWeight[edgeIndex][1]);
                edgeIndex++;
            }

            results[queryIndex] = components.IsConnected(query[0], query[1]);
        }

        return results;
    }
}
