using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Three Divisors (LC 1952): the textbook O(num) full-range trial division vs. this
// repo's own BinarySearch.LowerBound anchoring the scan at floor(sqrt(num)) - the same
// technique FourDivisorsBenchmarks/ClosestDivisorsBenchmarks use - then a short downward
// walk that bails out the moment a fourth divisor appears, so the common case (more or
// fewer than exactly three divisors) stops paying almost immediately instead of scanning
// to 1.
[MemoryDiagnoser]
public class ThreeDivisorsBenchmarks
{
    private const int RandomSeed = 1952;
    private const int MaxGeneratedNumber = 20_000;
    private const int TargetDivisorCount = 3;
    private const int DistinctDivisorPairCount = 2;

    // floor(sqrt(int.MaxValue)) + 1, keeps the divisor^2 comparison in
    // SquareExceedsSequence from overflowing.
    private const int MaxSafeDivisorBound = 46_341;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxGeneratedNumber)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRangeScan()
    {
        var total = 0;

        foreach (var num in _nums)
        {
            total += FullRangeHasExactlyThree(num) ? 1 : 0;
        }

        return total;
    }

    [Benchmark]
    public int BinarySearchAnchored()
    {
        var total = 0;

        foreach (var num in _nums)
        {
            total += AnchoredHasExactlyThree(num) ? 1 : 0;
        }

        return total;
    }

    private static bool FullRangeHasExactlyThree(int num)
    {
        var count = 0;

        for (var divisor = 1; divisor <= num; divisor++)
        {
            if (num % divisor != 0)
            {
                continue;
            }

            count++;

            if (count > TargetDivisorCount)
            {
                return false;
            }
        }

        return count == TargetDivisorCount;
    }

    private static bool AnchoredHasExactlyThree(int num)
    {
        var sequence = new SquareExceedsSequence(num, Math.Min(num, MaxSafeDivisorBound) + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        var count = 0;

        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            (count, var exceeded) = AccumulateDivisor(num, divisor, count);

            if (exceeded)
            {
                return false;
            }
        }

        return count == TargetDivisorCount;
    }

    private static (int Count, bool Exceeded) AccumulateDivisor(int num, int divisor, int count)
    {
        if (num % divisor != 0)
        {
            return (count, false);
        }

        var paired = num / divisor;
        count += divisor == paired ? 1 : DistinctDivisorPairCount;

        return (count, count > TargetDivisorCount);
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
