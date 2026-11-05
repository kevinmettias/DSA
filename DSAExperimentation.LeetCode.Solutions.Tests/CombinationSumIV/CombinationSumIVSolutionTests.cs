using DSAExperimentation.LeetCode.Solutions.CombinationSumIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CombinationSumIV;

// Harness only: both strategies live in CombinationSumIVSolution. One test method per
// strategy over one shared case, a zero target that only the empty combination
// reaches, so a failure names the strategy that broke. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class CombinationSumIVSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1], 0, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountCombinationsByTabulation_Cases_ReturnsOrderSensitiveCount(
        int[] nums, int target, int expected)
    {
        var actual = CombinationSumIVSolution.CountCombinationsByTabulation(nums, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountCombinationsByMemoizedRecursion_Cases_ReturnsOrderSensitiveCount(
        int[] nums, int target, int expected)
    {
        var actual = CombinationSumIVSolution.CountCombinationsByMemoizedRecursion(nums, target);

        Assert.Equal(expected, actual);
    }
}
