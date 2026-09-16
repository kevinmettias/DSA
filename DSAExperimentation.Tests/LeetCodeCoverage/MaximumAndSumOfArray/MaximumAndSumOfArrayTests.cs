using DSAExperimentation.LeetCode.MaximumAndSumOfArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumAndSumOfArray;

// Harness only: both strategies are MaximumAndSumOfArraySolution's - the unmemoized
// recursion and the same recursion routed through this repo's Memoizer - and this
// file just pins them to LeetCode's published examples plus the small cases that
// catch the two ways a slot walk goes wrong: forcing a slot to be filled, and
// filling slots in index order rather than picking the better AND.
public sealed class MaximumAndSumOfArrayTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, 6], 3, 9 },
            { [1, 3, 10, 4, 7, 1], 9, 24 },

            // 3 & 1 = 1 but 3 & 2 = 2 - picking slot 1 just because it comes first
            // would be wrong; the walk must consider leaving slot 1 empty.
            { [3], 2, 2 },

            // Both elements fit in the single slot, and both must be placed there.
            { [1, 1], 1, 2 },

            // Nothing ANDs usefully with slot 1, so the best achievable sum is 0.
            { [8], 1, 0 },

            // One element per slot wins here: 1 & 5 = 1 plus 2 & 6 = 2 beats pairing
            // both into slot 2, which scores only 0 + 2.
            { [5, 6], 2, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumAndSumByBruteForceRecursion_LeetCodeExamples_ReturnsMaximumAndSum(
        int[] nums, int numSlots, int expected)
    {
        var actual = MaximumAndSumOfArraySolution.MaximumAndSumByBruteForceRecursion(nums, numSlots);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumAndSumByMemoizedBitmask_LeetCodeExamples_ReturnsMaximumAndSum(
        int[] nums, int numSlots, int expected)
    {
        var actual = MaximumAndSumOfArraySolution.MaximumAndSumByMemoizedBitmask(nums, numSlots);

        Assert.Equal(expected, actual);
    }
}
