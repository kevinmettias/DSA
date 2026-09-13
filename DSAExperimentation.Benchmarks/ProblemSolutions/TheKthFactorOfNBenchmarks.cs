using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode;
using DSAExperimentation.LeetCode.TheKthFactorOfN;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TheKthFactorOfNSolution's, the same methods
// TheKthFactorOfNTests proves correct - the textbook full-range trial division from
// 1..n against BinarySearch.LowerBound anchoring at floor(sqrt(n)), the same technique
// FourDivisorsBenchmarks and ClosestDivisorsBenchmarks use. K is fixed above the
// largest possible divisor count for any n in range (840 has the most divisors below
// 1000, at 32) so both strategies are forced through their full worst-case scan
// instead of an early return on the first factor making brute force look artificially
// competitive - the same "unreachable target" idea TwoSumBenchmarks uses. The random
// numbers are built once in [GlobalSetup], so generation is not charged to either arm.
[MemoryDiagnoser]
public class TheKthFactorOfNBenchmarks
{
    private const int UnreachableK = 40;
    private const int RandomSeed = 1492; // LC problem number
    private const int ValueUpperBoundExclusive = 1_000;

    [Params(1_000, 10_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRangeScan()
    {
        var total = 0;

        foreach (var n in _values)
        {
            var factor = TheKthFactorOfNSolution.KthFactorByFullRangeScan(n, UnreachableK);
            total += factor == LeetCodeAnswer.None ? 0 : factor;
        }

        return total;
    }

    [Benchmark]
    public int BinarySearchAnchored()
    {
        var total = 0;

        foreach (var n in _values)
        {
            var factor = TheKthFactorOfNSolution.KthFactorByBinarySearchAnchor(n, UnreachableK);
            total += factor == LeetCodeAnswer.None ? 0 : factor;
        }

        return total;
    }
}
