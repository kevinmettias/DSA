using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Graph Connectivity With Threshold (LC 1627): the same divisor-sieve Union sweep
// run over a naive, unranked, uncompressed union-find (baseline - parent-pointer
// Find walks the chain with no shortcutting) vs. this repo's own DisjointSet, whose
// path compression + union-by-rank keep Find/Union at O(alpha(n)) amortized
// (NumberOfOperationsToMakeNetworkConnectedBenchmarks' own naive-vs-DisjointSet
// precedent, here isolating the union-find implementation itself rather than
// contrasting it against an unrelated DFS). A low threshold (CityCount / 20) keeps
// the sieve dense - most cities end up chained through many small divisors, which
// is exactly the pattern that makes an uncompressed parent chain degrade toward
// O(n) per Find.
[MemoryDiagnoser]
public class GraphConnectivityWithThresholdBenchmarks
{
    private const int ThresholdDivisor = 20;
    private const int RandomSeed = 1627; // LC problem number
    private const int FirstMultipleFactor = 2;

    [Params(500, 5_000)]
    public int CityCount;

    private int _threshold;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        _threshold = CityCount / ThresholdDivisor;

        var random = new Random(RandomSeed);
        _queries = Enumerable.Range(0, CityCount)
            .Select(_ => new[] { random.Next(1, CityCount + 1), random.Next(1, CityCount + 1) })
            .ToArray();
    }

    private static int[] BuildIdentityParents(int cityCount)
    {
        var parent = new int[cityCount + 1];

        for (var i = 0; i <= cityCount; i++)
        {
            parent[i] = i;
        }

        return parent;
    }

    private static int Find(int[] parent, int x)
    {
        while (parent[x] != x)
        {
            x = parent[x];
        }

        return x;
    }

    private static void Union(int[] parent, int a, int b)
    {
        var rootA = Find(parent, a);
        var rootB = Find(parent, b);

        if (rootA != rootB)
        {
            parent[rootA] = rootB;
        }
    }

    private void UnionByDivisors(int[] parent)
    {
        for (var divisor = _threshold + 1; divisor <= CityCount; divisor++)
        {
            for (var multiple = FirstMultipleFactor * divisor; multiple <= CityCount; multiple += divisor)
            {
                Union(parent, divisor, multiple);
            }
        }
    }

    private bool[] ComputeQueryResults(int[] parent)
    {
        var results = new bool[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            results[i] = Find(parent, _queries[i][0]) == Find(parent, _queries[i][1]);
        }

        return results;
    }

    [Benchmark(Baseline = true)]
    public bool[] NaiveUnionFind()
    {
        var parent = BuildIdentityParents(CityCount);
        UnionByDivisors(parent);
        return ComputeQueryResults(parent);
    }

    [Benchmark]
    public bool[] DisjointSetUnionFind()
    {
        var components = new DisjointSet(CityCount + 1);

        for (var divisor = _threshold + 1; divisor <= CityCount; divisor++)
        {
            for (var multiple = FirstMultipleFactor * divisor; multiple <= CityCount; multiple += divisor)
            {
                components.Union(divisor, multiple);
            }
        }

        var results = new bool[_queries.Length];
        for (var i = 0; i < _queries.Length; i++)
        {
            results[i] = components.IsConnected(_queries[i][0], _queries[i][1]);
        }

        return results;
    }
}
