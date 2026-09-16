using DSAExperimentation.LeetCode.SearchInRotatedSortedArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInRotatedSortedArrayII;

// Harness only: the algorithms live in SearchInRotatedSortedArrayIISolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed class SearchInRotatedSortedArrayIITests
{
    public static TheoryData<RotatedSearchExample> Examples =>
        new()
        {
            new RotatedSearchExample(Nums: [2, 5, 6, 0, 0, 1, 2], Target: 0, TargetIsPresent: true),
            new RotatedSearchExample(Nums: [2, 5, 6, 0, 0, 1, 2], Target: 3, TargetIsPresent: false),
            new RotatedSearchExample(Nums: [1, 0, 1, 1, 1], Target: 0, TargetIsPresent: true),
            new RotatedSearchExample(Nums: [1, 1, 1, 1, 1], Target: 2, TargetIsPresent: false),
            new RotatedSearchExample(Nums: [], Target: 5, TargetIsPresent: false),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasTargetByLinearScan_LeetCodeExamples_ReturnsWhetherTargetIsPresent(RotatedSearchExample example)
    {
        var actual = SearchInRotatedSortedArrayIISolution.HasTargetByLinearScan(example.Nums, example.Target);

        Assert.Equal(example.TargetIsPresent, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasTargetByTrimDuplicatesThenBinarySearch_LeetCodeExamples_ReturnsWhetherTargetIsPresent(
        RotatedSearchExample example)
    {
        var actual = SearchInRotatedSortedArrayIISolution.HasTargetByTrimDuplicatesThenBinarySearch(
            example.Nums, example.Target);

        Assert.Equal(example.TargetIsPresent, actual);
    }

    // One LeetCode example: the rotated, possibly duplicated array, the target to
    // find, and whether the target is present. The expectation is named rather than
    // carried by its position, so the row reads as an assertion instead of a `true`.
    public readonly record struct RotatedSearchExample(int[] Nums, int Target, bool TargetIsPresent);
}
