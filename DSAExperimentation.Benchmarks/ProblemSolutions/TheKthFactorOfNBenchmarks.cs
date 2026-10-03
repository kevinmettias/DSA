using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TheKthFactorOfN;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TheKthFactorOfNSolution's, the same methods
// TheKthFactorOfNSolutionTests proves correct - the textbook full-range trial division from
// 1..n against BinarySearch.LowerBound anchoring at floor(sqrt(n)), the same technique
// FourDivisorsBenchmarks and ClosestDivisorsBenchmarks use. K is fixed above the
// largest possible divisor count for any n in range (840 has the most divisors below
// 1000, at 32) so both strategies are forced through their full worst-case scan
// instead of an early return on the first factor making brute force look artificially
// competitive - the same "unreachable target" idea TwoSumBenchmarks uses. The random
// numbers are built once in [GlobalSetup], so generation is not charged to either arm.
public class TheKthFactorOfNBenchmarks
{
    private const int UnreachableK = 40;
    private const int RandomSeed = 1492; // LC problem number
    private const int ValueUpperBoundExclusive = 1_000;

    private int[] _values = [];

    // Every number's k-th factor, or -1 when it has fewer, in input order - what each arm returns.
    private int[] _factors = [];

    [Params(1_000, 10_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, 1, ValueUpperBoundExclusive, random);
        _factors = new int[_values.Length];
    }

    [Benchmark(Baseline = true)]
    public int[] FullRangeScan()
    {
        for (var i = 0; i < _values.Length; i++)
        {
            _factors[i] = TheKthFactorOfNSolution.KthFactorByFullRangeScan(_values[i], UnreachableK);
        }

        return _factors;
    }

    [Benchmark]
    public int[] BinarySearchAnchored()
    {
        for (var i = 0; i < _values.Length; i++)
        {
            _factors[i] = TheKthFactorOfNSolution.KthFactorByBinarySearchAnchor(_values[i], UnreachableK);
        }

        return _factors;
    }
}
