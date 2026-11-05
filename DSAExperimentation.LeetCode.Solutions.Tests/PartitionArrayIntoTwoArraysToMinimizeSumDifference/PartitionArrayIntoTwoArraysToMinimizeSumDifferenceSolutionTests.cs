using DSAExperimentation.LeetCode.Solutions.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

// Harness only. Two cases pin the equal-size constraint the meet-in-the-middle
// grouping exists for: a run of identical values, and one where the balanced split
// is not the one a size-blind subset search would pick. Both strategies live in
// PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution and are asserted
// separately so a failure names the arm that broke. LeetCode 2035's published
// examples run in CatalogCaseTests.
public sealed partial class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolutionTests
{
    public static TheoryData<int[], int> Cases => new()
    {
        { [5, 5, 5, 5], 0 },
        { [1, 2, 3, 100], 96 },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumDifferenceByBruteForceEqualSplits_Cases_ReturnsMinimumAchievableSumDifference(
        int[] nums, int expected)
    {
        var actual = PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByBruteForceEqualSplits(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumDifferenceByMeetInTheMiddle_Cases_ReturnsMinimumAchievableSumDifference(
        int[] nums, int expected)
    {
        var actual = PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByMeetInTheMiddle(nums);

        Assert.Equal(expected, actual);
    }
}
