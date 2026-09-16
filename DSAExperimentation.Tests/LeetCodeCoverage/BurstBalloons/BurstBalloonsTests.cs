using DSAExperimentation.LeetCode.BurstBalloons;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BurstBalloons;

// Harness only: the algorithms live in BurstBalloonsSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed partial class BurstBalloonsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 1, 5, 8], 167 },
            { [1, 5], 10 },
            { [7], 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCoinsByUnmemoizedRecursion_LeetCodeExamples_ReturnsMaximumCoinsFromBursting(
        int[] nums, int expected) =>
        Assert.Equal(expected, BurstBalloonsSolution.MaxCoinsByUnmemoizedRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCoinsByMemoizedRecursion_LeetCodeExamples_ReturnsMaximumCoinsFromBursting(
        int[] nums, int expected) =>
        Assert.Equal(expected, BurstBalloonsSolution.MaxCoinsByMemoizedRecursion(nums));
}
