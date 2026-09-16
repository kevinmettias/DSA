using DSAExperimentation.LeetCode.CombinationSumIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CombinationSumIV;

// Harness only: both strategies live in CombinationSumIVSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed partial class CombinationSumIVTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 3], 4, 7 },
            { [9], 3, 0 },
            { [1], 0, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCombinationsByTabulation_LeetCodeExamples_ReturnsOrderSensitiveCount(
        int[] nums, int target, int expected)
    {
        var actual = CombinationSumIVSolution.CountCombinationsByTabulation(nums, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCombinationsByMemoizedRecursion_LeetCodeExamples_ReturnsOrderSensitiveCount(
        int[] nums, int target, int expected)
    {
        var actual = CombinationSumIVSolution.CountCombinationsByMemoizedRecursion(nums, target);

        Assert.Equal(expected, actual);
    }
}
