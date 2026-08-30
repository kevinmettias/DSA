using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestDivisors;

// LeetCode 1362. Closest Divisors: for each of num+1/num+2, BinarySearch.LowerBound
// locates floor(sqrt(candidate)) over a monotone virtual sequence the same way
// SqrtXTests does for LC 69, then a short walk downward finds the first exact
// divisor - the largest divisor <= sqrt(candidate), whose paired divisor is
// necessarily the closest possible pair for that candidate.
public sealed partial class ClosestDivisorsTests
{
    [Theory]
    [InlineData(8, 3, 3)]
    [InlineData(123, 5, 25)]
    [InlineData(999, 25, 40)]
    public void ClosestDivisors_LeetCodeExamples_ReturnsClosestPair(int num, int expectedFirst, int expectedSecond)
    {
        var (first, second) = FindClosestDivisors(num);

        Assert.Equal(expectedFirst, first);
        Assert.Equal(expectedSecond, second);
    }

    private static (int First, int Second) FindClosestDivisors(int num)
    {
        var lower = ClosestPairFor(num + 1);
        var upper = ClosestPairFor(num + 2);

        return upper.Second - upper.First < lower.Second - lower.First ? upper : lower;
    }

    private static (int First, int Second) ClosestPairFor(int candidate)
    {
        var sequence = new SquareExceedsSequence(candidate, Math.Min(candidate, 46_341) + 1);
        var divisor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        while (candidate % divisor != 0)
        {
            divisor--;
        }

        return (divisor, candidate / divisor);
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
