using DSAExperimentation.LeetCode.ClosestSubsequenceSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestSubsequenceSum;

// Harness only: both strategies live in ClosestSubsequenceSumSolution and are
// asserted against the same examples, so a failure names the strategy that broke.
// The full 2^n subset scan was previously only a benchmark's baseline arm and went
// unasserted; it is under test here for the first time, against the same meet-in-
// the-middle answers.
public sealed class ClosestSubsequenceSumTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [5, -7, 3, 5], 6, 0 },
            { [7, -9, 15, -2], -5, 1 },
            { [1, 2, 3], -7, 7 },
            { [-1, -2, -3], -4, 0 },
            { [5], 3, 2 },
            { [1], 0, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAbsDifferenceByBruteForceSubsets_LeetCodeExamples_ReturnsClosestAchievableSum(
        int[] nums, int goal, int expected)
    {
        var actual = ClosestSubsequenceSumSolution.MinAbsDifferenceByBruteForceSubsets(nums, goal);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAbsDifferenceByMeetInTheMiddle_LeetCodeExamples_ReturnsClosestAchievableSum(
        int[] nums, int goal, int expected)
    {
        var actual = ClosestSubsequenceSumSolution.MinAbsDifferenceByMeetInTheMiddle(nums, goal);

        Assert.Equal(expected, actual);
    }
}
