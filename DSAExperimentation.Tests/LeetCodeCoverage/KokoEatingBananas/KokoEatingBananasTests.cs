using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KokoEatingBananas;

// LeetCode 875. Koko Eating Bananas: "binary search on the answer" over eating
// speed - the minimum speed k that lets Koko finish every pile within h hours is
// monotone (once some speed is feasible, every faster speed stays feasible), so
// it's the leftmost "true" in an implicit [false...false, true...true] sequence
// over k in [1, max(piles)]. Same FeasibleSpeedSequence + BinarySearch.LowerBound
// shape SplitArrayLargestSumTests already uses for its own search-on-answer.
public sealed partial class KokoEatingBananasTests
{
    [Theory]
    [InlineData(new[] { 3, 6, 7, 11 }, 8, 4)]
    [InlineData(new[] { 30, 11, 23, 4, 20 }, 5, 30)]
    [InlineData(new[] { 30, 11, 23, 4, 20 }, 6, 23)]
    [InlineData(new[] { 1 }, 1, 1)]
    public void MinEatingSpeed_LeetCodeExamples_ReturnsSmallestFeasibleSpeed(int[] piles, int h, int expected)
    {
        var actual = MinEatingSpeed(piles, h);
        Assert.Equal(expected, actual);
    }

    private static int MinEatingSpeed(int[] piles, int h)
    {
        var sequence = new FeasibleSpeedSequence(piles, h);
        return 1 + BinarySearch.LowerBound(sequence, true);
    }

    private static long HoursNeeded(int[] piles, int speed)
    {
        var hours = 0L;

        foreach (var pile in piles)
        {
            hours += (pile + speed - 1) / speed;
        }

        return hours;
    }

    private readonly struct FeasibleSpeedSequence(int[] piles, int h) : IRandomAccessSequence<bool>
    {
        public int Length => piles.Max();

        public bool Get(int index) => HoursNeeded(piles, 1 + index) <= h;
    }
}
