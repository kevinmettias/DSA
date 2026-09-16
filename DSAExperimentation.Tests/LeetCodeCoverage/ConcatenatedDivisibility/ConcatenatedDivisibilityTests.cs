using DSAExperimentation.LeetCode.ConcatenatedDivisibility;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConcatenatedDivisibility;

// Harness only: both strategies live in ConcatenatedDivisibilitySolution and are
// asserted against the same examples, so a failure names the strategy that broke.
public sealed class ConcatenatedDivisibilityTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [3, 12, 45], 5, [3, 12, 45] },
            { [10, 5], 10, [5, 10] },
            { [1, 2, 3], 5, [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestPermutationByBacktracking_LeetCodeExamples_ReturnsLexicographicallySmallestDivisibleOrder(
        int[] nums, int k, int[] expected)
    {
        var actual = ConcatenatedDivisibilitySolution.SmallestPermutationByBacktracking(nums, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestPermutationByBitmaskMemo_LeetCodeExamples_ReturnsLexicographicallySmallestDivisibleOrder(
        int[] nums, int k, int[] expected)
    {
        var actual = ConcatenatedDivisibilitySolution.SmallestPermutationByBitmaskMemo(nums, k);

        Assert.Equal(expected, actual);
    }
}
