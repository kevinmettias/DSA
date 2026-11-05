using DSAExperimentation.LeetCode.Solutions.BurstBalloons;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BurstBalloons;

// Harness only: the algorithms live in BurstBalloonsSolution. One test method per
// strategy over one shared set of cases, so a failure names the strategy that broke.
// The remaining case is a single balloon, whose coins are its own value. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class BurstBalloonsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [7], 7 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxCoinsByUnmemoizedRecursion_Cases_ReturnsMaximumCoinsFromBursting(
        int[] nums, int expected) =>
        Assert.Equal(expected, BurstBalloonsSolution.MaxCoinsByUnmemoizedRecursion(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxCoinsByMemoizedRecursion_Cases_ReturnsMaximumCoinsFromBursting(
        int[] nums, int expected) =>
        Assert.Equal(expected, BurstBalloonsSolution.MaxCoinsByMemoizedRecursion(nums));
}
