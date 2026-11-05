using DSAExperimentation.LeetCode.Solutions.FindMinimumInRotatedSortedArrayII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindMinimumInRotatedSortedArrayII;

// Harness only: the algorithms live in FindMinimumInRotatedSortedArrayIISolution.
// One test method per strategy over one shared set of cases, so a failure names the
// strategy that broke. The case is an array of one repeated value, where every
// comparison the binary shrink makes is a tie. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class FindMinimumInRotatedSortedArrayIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 2, 2, 2], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMinByLinearScan_Cases_ReturnsMinimum(int[] nums, int expected) =>
        Assert.Equal(expected, FindMinimumInRotatedSortedArrayIISolution.FindMinByLinearScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMinByDuplicateTolerantBinaryShrink_Cases_ReturnsMinimum(int[] nums, int expected) =>
        Assert.Equal(expected, FindMinimumInRotatedSortedArrayIISolution.FindMinByDuplicateTolerantBinaryShrink(nums));
}
