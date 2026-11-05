using DSAExperimentation.LeetCode.Solutions.CountOfSmallerNumbersAfterSelf;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountOfSmallerNumbersAfterSelf;

// Harness only: the algorithms live in CountOfSmallerNumbersAfterSelfSolution. One
// test method per strategy over one shared set of cases, so a failure names the
// strategy that broke. The case is an array of equal values, where no element to the
// right is strictly smaller. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountOfSmallerNumbersAfterSelfSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [1, 1, 1], [0, 0, 0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSmallerByPairwiseScan_Cases_ReturnsCountsToTheRight(int[] nums, int[] expected) =>
        Assert.Equal(expected, CountOfSmallerNumbersAfterSelfSolution.CountSmallerByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSmallerByFenwickTreeSweep_Cases_ReturnsCountsToTheRight(int[] nums, int[] expected) =>
        Assert.Equal(expected, CountOfSmallerNumbersAfterSelfSolution.CountSmallerByFenwickTreeSweep(nums));
}
