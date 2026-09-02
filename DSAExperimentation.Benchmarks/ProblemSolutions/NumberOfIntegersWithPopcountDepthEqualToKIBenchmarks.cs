using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfIntegersWithPopcountDepthEqualToKISolution's,
// the same methods NumberOfIntegersWithPopcountDepthEqualToKITests proves
// correct (CountKReducibleNumbersLessThanNBenchmarks precedent). N stays small
// - brute force simulates every one of the N integers it counts, so it would
// not finish at the real problem's n up to 10^15, even though the
// combinatorial arm scales to it trivially. No [GlobalSetup] is needed: unlike
// a graph or a random string, N and K are the LeetCode input themselves, with
// nothing to charge to setup.
[MemoryDiagnoser]
public class NumberOfIntegersWithPopcountDepthEqualToKIBenchmarks
{
    private const int K = 2;

    [Params(10_000, 1_000_000)]
    public long N;

    [Benchmark(Baseline = true)]
    public long BruteForce() => NumberOfIntegersWithPopcountDepthEqualToKISolution.PopcountDepthByBruteForce(N, K);

    [Benchmark]
    public long PopcountCombinatorics() =>
        NumberOfIntegersWithPopcountDepthEqualToKISolution.PopcountDepthByPopcountCombinatorics(N, K);
}
