using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNonZeroProductOfTheArrayElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNonZeroProductOfTheArrayElementsSolution's, the
// same methods MinimumNonZeroProductOfTheArrayElementsTests proves correct. The
// parameter is LeetCode's own p rather than a stand-in exponent, so both arms compute
// the real answer; p = 15 and p = 21 put the pair count at 16,383 and 1,048,575, the
// 10^4 / 10^6 workload sizes this comparison was always run at. That is as large as
// the naive arm can finish - LeetCode's own p <= 60 means an exponent up to 2^59,
// which is exactly why the squaring strategy exists at all.
[MemoryDiagnoser]
public class MinimumNonZeroProductOfTheArrayElementsBenchmarks
{
    [Params(15, 21)]
    public int Power { get; set; }

    [Benchmark(Baseline = true)]
    public long RepeatedModularMultiplication() =>
        MinimumNonZeroProductOfTheArrayElementsSolution.MinNonZeroProductByRepeatedMultiplication(Power);

    [Benchmark]
    public long ModPowBySquaring() =>
        MinimumNonZeroProductOfTheArrayElementsSolution.MinNonZeroProductBySquaring(Power);
}
