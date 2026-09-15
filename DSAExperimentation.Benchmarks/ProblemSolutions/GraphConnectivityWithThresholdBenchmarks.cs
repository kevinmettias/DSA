using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GraphConnectivityWithThreshold;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GraphConnectivityWithThresholdSolution's, the same
// methods GraphConnectivityWithThresholdTests proves correct - the divisor-sieve Union
// sweep run over a naive, unranked, uncompressed union-find (baseline) vs. this repo's
// own DisjointSet, whose path compression + union-by-rank keep Find/Union at
// O(alpha(n)) amortized (NumberOfOperationsToMakeNetworkConnectedBenchmarks' own
// naive-vs-DisjointSet precedent, here isolating the union-find implementation itself
// rather than contrasting it against an unrelated DFS). A low threshold
// (CityCount / 20) keeps the sieve dense - most cities end up chained through many
// small divisors, which is exactly the pattern that makes an uncompressed parent chain
// degrade toward O(n) per Find.
[MemoryDiagnoser]
public class GraphConnectivityWithThresholdBenchmarks
{
    private const int ThresholdDivisor = 20;
    private const int RandomSeed = 1627; private int _threshold;

    private int[][] _queries = [];
    // LC problem number

    [Params(500, 5_000)]
    public int CityCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _threshold = CityCount / ThresholdDivisor;

        var random = new Random(RandomSeed);
        _queries = Enumerable.Range(0, CityCount)
            .Select(_ => new[] { random.Next(1, CityCount + 1), random.Next(1, CityCount + 1) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool[] NaiveUnionFind() =>
        GraphConnectivityWithThresholdSolution.AreConnectedByNaiveUnionFind(CityCount, _threshold, _queries);

    [Benchmark]
    public bool[] DisjointSetUnionFind() =>
        GraphConnectivityWithThresholdSolution.AreConnectedByDisjointSet(CityCount, _threshold, _queries);
}
