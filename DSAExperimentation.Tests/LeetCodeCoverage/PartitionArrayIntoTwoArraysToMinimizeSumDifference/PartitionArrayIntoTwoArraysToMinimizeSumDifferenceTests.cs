using DSAExperimentation.LeetCode.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

// Harness only. LeetCode 2035's three published examples plus two that pin the
// equal-size constraint the meet-in-the-middle grouping exists for: a run of
// identical values, and one where the balanced split is not the one a size-blind
// subset search would pick. Both strategies live in
// PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution and are asserted
// separately so a failure names the arm that broke.
public sealed class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceTests
{
    public static TheoryData<int[], int> Examples => new()
    {
        { [3, 9, 7, 3], 2 },
        { [-36, 36], 72 },
        { [2, -1, 0, 4, -2, -9], 0 },
        { [5, 5, 5, 5], 0 },
        { [1, 2, 3, 100], 96 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumDifferenceByBruteForceEqualSplits_LeetCodeExamples_ReturnsMinimumAchievableSumDifference(
        int[] nums, int expected)
    {
        var actual = PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByBruteForceEqualSplits(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumDifferenceByMeetInTheMiddle_LeetCodeExamples_ReturnsMinimumAchievableSumDifference(
        int[] nums, int expected)
    {
        var actual = PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByMeetInTheMiddle(nums);

        Assert.Equal(expected, actual);
    }
}
