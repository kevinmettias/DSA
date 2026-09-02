using DSAExperimentation.LeetCode.FindNthSmallestIntegerWithKOneBits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindNthSmallestIntegerWithKOneBits;

// Harness only: both strategies live in FindNthSmallestIntegerWithKOneBitsSolution
// and are asserted against the same examples, so a failure names the strategy
// that broke.
public sealed class FindNthSmallestIntegerWithKOneBitsTests
{
    public static TheoryData<long, int, long> Examples =>
        new()
        {
            { 4, 2, 9 },
            { 3, 1, 4 },
            { 1, 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthSmallestByPopCountScan_LeetCodeExamples_ReturnsNthIntegerWithKOneBits(long n, int k, long expected) =>
        Assert.Equal(expected, FindNthSmallestIntegerWithKOneBitsSolution.NthSmallestByPopCountScan(n, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthSmallestByMemoizedBinomialSelection_LeetCodeExamples_ReturnsNthIntegerWithKOneBits(long n, int k, long expected) =>
        Assert.Equal(expected, FindNthSmallestIntegerWithKOneBitsSolution.NthSmallestByMemoizedBinomialSelection(n, k));
}
