using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountGoodNumbers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountGoodNumbersSolution's, the same methods
// CountGoodNumbersTests proves correct. N is a scalar, so there is no input to
// prepare in a [GlobalSetup] - the two [Params] lengths are the whole workload, and
// they are what separates the baseline's O(n) multiplications from the squaring
// arm's O(log n).
[MemoryDiagnoser]
public class CountGoodNumbersBenchmarks
{
    [Params(1_000, 1_000_000)]
    public long N { get; set; }

    [Benchmark(Baseline = true)]
    public int RepeatedMultiplication() =>
        CountGoodNumbersSolution.CountGoodNumbersByRepeatedMultiplication(N);

    [Benchmark]
    public int ExponentiationBySquaring() =>
        CountGoodNumbersSolution.CountGoodNumbersByExponentiationBySquaring(N);
}
