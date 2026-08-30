using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Four Divisors (LC 1390): the textbook O(num) full-range trial division per
// number vs. this repo's own BinarySearch.LowerBound anchoring each number's scan
// at floor(sqrt(num)) - the same technique ClosestDivisorsBenchmarks uses for LC
// 1362 - then a short downward walk that bails out the moment a fifth divisor
// appears, so numbers with more than four divisors (the common case for random
// input) stop paying almost immediately instead of scanning to 1.
[MemoryDiagnoser]
public class FourDivisorsBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1390);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, 20_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRangeScan()
    {
        var total = 0;

        foreach (var num in _nums)
        {
            total += FullRangeDivisorSumIfExactlyFour(num);
        }

        return total;
    }

    [Benchmark]
    public int BinarySearchAnchored()
    {
        var total = 0;

        foreach (var num in _nums)
        {
            total += AnchoredDivisorSumIfExactlyFour(num);
        }

        return total;
    }

    private static int FullRangeDivisorSumIfExactlyFour(int num)
    {
        var count = 0;
        var sum = 0;

        for (var divisor = 1; divisor <= num; divisor++)
        {
            if (num % divisor != 0)
            {
                continue;
            }

            count++;
            sum += divisor;

            if (count > 4)
            {
                return 0;
            }
        }

        return count == 4 ? sum : 0;
    }

    private static int AnchoredDivisorSumIfExactlyFour(int num)
    {
        var sequence = new SquareExceedsSequence(num, Math.Min(num, 46_341) + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        var count = 0;
        var sum = 0;

        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (num % divisor != 0)
            {
                continue;
            }

            var paired = num / divisor;
            count += divisor == paired ? 1 : 2;
            sum += divisor == paired ? divisor : divisor + paired;

            if (count > 4)
            {
                return 0;
            }
        }

        return count == 4 ? sum : 0;
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
