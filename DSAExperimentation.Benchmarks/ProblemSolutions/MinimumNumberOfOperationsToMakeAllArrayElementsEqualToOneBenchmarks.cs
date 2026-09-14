using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution's, the same
// methods MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneTests proves
// correct. LC 2654 scans every (start, end) window for the shortest one whose own
// running gcd hits 1 - the same gcd-fold shape CheckIfItIsAGoodArrayBenchmarks and
// FindGreatestCommonDivisorOfArrayBenchmarks already measure, just repeated once
// per window instead of once for the whole array - so the performance question
// left is how each pairwise gcd step is computed: repeated subtraction (textbook,
// O(max/min) per pair) against the modulo-based Euclidean algorithm (O(log min)
// per pair). _values are all multiples of MultipleFactor so no window's running
// gcd ever reaches 1, forcing both arms through the full O(n^2) scan instead of
// one short-circuiting on an early window.
[MemoryDiagnoser]
public class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneBenchmarks
{
    private const int RandomSeed = 2654; // LC problem number
    private const int MultipleFactor = 6; // every value stays a multiple of 6, so no window's gcd ever reaches 1
    private const int MaxRandomValueExclusive = 5_000;

    [Params(30, 100)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => MultipleFactor * random.Next(1, MaxRandomValueExclusive))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int MinOperationsBySubtractionGcd() =>
        MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution.MinOperationsBySubtractionGcd(_values);

    [Benchmark]
    public int MinOperationsByEuclideanGcd() =>
        MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution.MinOperationsByEuclideanGcd(_values);
}
