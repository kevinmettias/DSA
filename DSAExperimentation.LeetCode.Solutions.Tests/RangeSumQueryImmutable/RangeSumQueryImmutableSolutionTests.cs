using DSAExperimentation.LeetCode.Solutions.RangeSumQueryImmutable;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RangeSumQueryImmutable;

// Harness only. All three strategies are RangeSumQueryImmutableSolution's - this row
// asks the NumArray each one's factory makes for a one-element window of a negative
// value inside a three-element array. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class RangeSumQueryImmutableSolutionTests
{
    public static TheoryData<int[], int, int, int> Cases =>
        new()
        {
            { new[] { 7, -3, 4 }, 1, 1, -3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumRangeByFenwickTree_Cases_ReturnsRangeSum(int[] nums, int left, int right, int expected)
    {
        var numArray = RangeSumQueryImmutableSolution.SumRangeByFenwickTree(nums);
        var actual = numArray.SumRange(left, right);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumRangeByPrefixSums_Cases_ReturnsRangeSum(int[] nums, int left, int right, int expected)
    {
        var numArray = RangeSumQueryImmutableSolution.SumRangeByPrefixSums(nums);
        var actual = numArray.SumRange(left, right);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumRangeByBruteForceRescan_Cases_ReturnsRangeSum(int[] nums, int left, int right, int expected)
    {
        var numArray = RangeSumQueryImmutableSolution.SumRangeByBruteForceRescan(nums);
        var actual = numArray.SumRange(left, right);

        Assert.Equal(expected, actual);
    }
}
