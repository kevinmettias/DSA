using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfGroupsWithIncreasingLength;

// LeetCode 2790. Maximum Number of Groups With Increasing Length: sort usageLimits with
// this repo's own MergeSort (over an ArrayIndexedSequence<int>), then sweep ascending
// while accumulating a running "available uses" total. Whenever that total reaches the
// next group's required size, one more group is feasible - claim it and subtract the
// size from the pool. This is the standard exchange-argument greedy: processing small
// limits first means an index with little capacity gets "spent" on early, small groups
// while high-capacity indices carry over to sustain later, larger ones.
public sealed partial class MaximumNumberOfGroupsWithIncreasingLengthTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 5 }, 3)]
    [InlineData(new[] { 2, 1, 2 }, 2)]
    [InlineData(new[] { 1, 1 }, 1)]
    public void MaxIncreasingGroups_LeetCodeExamples_ReturnsExpectedGroupCount(int[] usageLimits, int expected)
        => Assert.Equal(expected, MaxIncreasingGroups(usageLimits));

    [Fact]
    public void MaxIncreasingGroups_SingleIndexWithSpareCapacity_OnlyOneGroupPossible()
        => Assert.Equal(1, MaxIncreasingGroups([5]));

    private static int MaxIncreasingGroups(int[] usageLimits)
    {
        var sorted = (int[])usageLimits.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        long available = 0;
        var groups = 0;
        var neededSize = 1;

        foreach (var limit in sorted)
        {
            available += limit;

            if (available >= neededSize)
            {
                groups++;
                available -= neededSize;
                neededSize++;
            }
        }

        return groups;
    }
}
