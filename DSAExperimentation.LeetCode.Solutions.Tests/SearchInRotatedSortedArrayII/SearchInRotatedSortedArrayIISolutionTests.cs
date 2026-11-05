using DSAExperimentation.LeetCode.Solutions.SearchInRotatedSortedArrayII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchInRotatedSortedArrayII;

// Harness only: the algorithms live in SearchInRotatedSortedArrayIISolution. One
// test method per strategy over one shared set of cases, so a failure names the
// strategy that broke. The cases are an array of one repeated value that lacks the
// target, and an empty array. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SearchInRotatedSortedArrayIISolutionTests
{
    public static TheoryData<RotatedSearchCase> Cases =>
        new()
        {
            new RotatedSearchCase(Nums: [1, 1, 1, 1, 1], Target: 2, TargetIsPresent: false),
            new RotatedSearchCase(Nums: [], Target: 5, TargetIsPresent: false),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasTargetByLinearScan_Cases_ReturnsWhetherTargetIsPresent(RotatedSearchCase testCase)
    {
        var actual = SearchInRotatedSortedArrayIISolution.HasTargetByLinearScan(testCase.Nums, testCase.Target);

        Assert.Equal(testCase.TargetIsPresent, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasTargetByTrimDuplicatesThenBinarySearch_Cases_ReturnsWhetherTargetIsPresent(
        RotatedSearchCase testCase)
    {
        var actual = SearchInRotatedSortedArrayIISolution.HasTargetByTrimDuplicatesThenBinarySearch(
            testCase.Nums, testCase.Target);

        Assert.Equal(testCase.TargetIsPresent, actual);
    }

    // One case: the rotated, possibly duplicated array, the target to
    // find, and whether the target is present. The expectation is named rather than
    // carried by its position, so the row reads as an assertion instead of a `true`.
    public readonly record struct RotatedSearchCase(int[] Nums, int Target, bool TargetIsPresent);
}
