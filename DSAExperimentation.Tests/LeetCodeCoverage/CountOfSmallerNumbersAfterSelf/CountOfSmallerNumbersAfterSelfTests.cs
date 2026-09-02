using DSAExperimentation.LeetCode.CountOfSmallerNumbersAfterSelf;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountOfSmallerNumbersAfterSelf;

// Harness only: the algorithms live in CountOfSmallerNumbersAfterSelfSolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed class CountOfSmallerNumbersAfterSelfTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [5, 2, 6, 1], [2, 1, 1, 0] },
            { [1, 1, 1], [0, 0, 0] },
            { [3, 2, 1], [2, 1, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSmallerByPairwiseScan_LeetCodeExamples_ReturnsCountsToTheRight(int[] nums, int[] expected) =>
        Assert.Equal(expected, CountOfSmallerNumbersAfterSelfSolution.CountSmallerByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSmallerByFenwickTreeSweep_LeetCodeExamples_ReturnsCountsToTheRight(int[] nums, int[] expected) =>
        Assert.Equal(expected, CountOfSmallerNumbersAfterSelfSolution.CountSmallerByFenwickTreeSweep(nums));
}
