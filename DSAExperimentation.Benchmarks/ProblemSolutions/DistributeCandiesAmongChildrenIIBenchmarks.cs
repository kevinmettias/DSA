using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistributeCandiesAmongChildrenIISolution's, the same
// methods DistributeCandiesAmongChildrenIITests proves correct. The double loop's
// true cost is driven by Limit (its bound is min(CandyCount, Limit) outer,
// min(CandyCount - first, Limit) inner) - once CandyCount comfortably exceeds
// 3*Limit neither loop is truncated early by the candy count, so Limit alone
// controls the brute-force workload, matching LC 2929's real up-to-1e6 domain far
// better than scaling the candy count would.
[MemoryDiagnoser]
public class DistributeCandiesAmongChildrenIIBenchmarks
{
    private int CandyCount => 4 * Limit;

    [Params(200, 2_000)]
    public int Limit { get; set; }

    [Benchmark(Baseline = true)]
    public long BruteForce() => DistributeCandiesAmongChildrenIISolution.CountWaysByBruteForce(CandyCount, Limit);

    [Benchmark]
    public long InclusionExclusion() => DistributeCandiesAmongChildrenIISolution.CountWaysByInclusionExclusion(CandyCount, Limit);
}
