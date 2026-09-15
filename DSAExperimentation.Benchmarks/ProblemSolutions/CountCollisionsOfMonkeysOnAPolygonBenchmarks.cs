using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountCollisionsOfMonkeysOnAPolygon;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountCollisionsOfMonkeysOnAPolygonSolution's, the
// same methods CountCollisionsOfMonkeysOnAPolygonTests proves correct. The answer
// is always (2^n - 2) mod 1e9+7, so the only performance question is how 2^n is
// raised - naive repeated multiplication (O(n)) vs. exponentiation by squaring
// (O(log n)). N is a scalar, so there is no input to prepare in a [GlobalSetup]:
// the two [Params] lengths are the whole workload.
[MemoryDiagnoser]
public class CountCollisionsOfMonkeysOnAPolygonBenchmarks
{
    [Params(1_000, 1_000_000)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public int RepeatedMultiplication() =>
        CountCollisionsOfMonkeysOnAPolygonSolution.NumberOfWaysByRepeatedMultiplication(N);

    [Benchmark]
    public int ExponentiationBySquaring() =>
        CountCollisionsOfMonkeysOnAPolygonSolution.NumberOfWaysByExponentiationBySquaring(N);
}
