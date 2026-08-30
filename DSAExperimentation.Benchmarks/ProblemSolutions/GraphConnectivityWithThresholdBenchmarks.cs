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
    [Params(500, 5_000)]
    public int CityCount;

    private int _threshold;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        _threshold = CityCount / 20;

        var random = new Random(1627);
        _queries = Enumerable.Range(0, CityCount)
            .Select(_ => new[] { random.Next(1, CityCount + 1), random.Next(1, CityCount + 1) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool[] NaiveUnionFind()
    {
        var parent = new int[CityCount + 1];
        for (var i = 0; i <= CityCount; i++)
        {
            parent[i] = i;
        }

        int Find(int x)
        {
            while (parent[x] != x)
            {
                x = parent[x];
            }

            return x;
        }

        void Union(int a, int b)
        {
            var rootA = Find(a);
            var rootB = Find(b);

            if (rootA != rootB)
            {
                parent[rootA] = rootB;
            }
        }

        for (var divisor = _threshold + 1; divisor <= CityCount; divisor++)
        {
            for (var multiple = 2 * divisor; multiple <= CityCount; multiple += divisor)
            {
                Union(divisor, multiple);
            }
        }

        var results = new bool[_queries.Length];
        for (var i = 0; i < _queries.Length; i++)
        {
            results[i] = Find(_queries[i][0]) == Find(_queries[i][1]);
        }

        return results;
    }

    [Benchmark]
    public bool[] DisjointSetUnionFind()
    {
        var components = new DisjointSet(CityCount + 1);

        for (var divisor = _threshold + 1; divisor <= CityCount; divisor++)
        {
            for (var multiple = 2 * divisor; multiple <= CityCount; multiple += divisor)
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
