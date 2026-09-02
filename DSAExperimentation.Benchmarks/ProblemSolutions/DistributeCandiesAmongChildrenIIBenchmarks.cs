using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistributeCandiesAmongChildrenIISolution's, the same
// methods DistributeCandiesAmongChildrenIITests proves correct. The double loop's
// true cost is driven by limit (its bound is min(n, limit) outer, min(n-first,
// limit) inner) - once N comfortably exceeds 3*Limit neither loop is truncated
// early by n, so Limit alone controls the brute-force workload, matching LC
// 2929's real up-to-1e6 domain far better than scaling n would.
[MemoryDiagnoser]
public class DistributeCandiesAmongChildrenIIBenchmarks
{
    [Params(200, 2_000)]
    public int Limit;

    private int N => 4 * Limit;

    [Benchmark(Baseline = true)]
    public long BruteForce() => DistributeCandiesAmongChildrenIISolution.CountWaysByBruteForce(N, Limit);

    [Benchmark]
    public long InclusionExclusion() => DistributeCandiesAmongChildrenIISolution.CountWaysByInclusionExclusion(N, Limit);
}
