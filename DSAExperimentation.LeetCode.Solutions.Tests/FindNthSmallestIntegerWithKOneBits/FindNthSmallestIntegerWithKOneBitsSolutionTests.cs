using DSAExperimentation.LeetCode.Solutions.FindNthSmallestIntegerWithKOneBits;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindNthSmallestIntegerWithKOneBits;

// Harness only: both strategies live in FindNthSmallestIntegerWithKOneBitsSolution
// and are asserted against the same cases, so a failure names the strategy
// that broke.
public sealed partial class FindNthSmallestIntegerWithKOneBitsSolutionTests
{
    public static TheoryData<long, int, long> Cases =>
        new()
        {
            { 1, 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NthSmallestByPopCountScan_Cases_ReturnsNthIntegerWithKOneBits(
        long position, int oneBitCount, long expected)
    {
        var actual = FindNthSmallestIntegerWithKOneBitsSolution.NthSmallestByPopCountScan(position, oneBitCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void NthSmallestByMemoizedBinomialSelection_Cases_ReturnsNthIntegerWithKOneBits(
        long position, int oneBitCount, long expected)
    {
        var actual = FindNthSmallestIntegerWithKOneBitsSolution.NthSmallestByMemoizedBinomialSelection(position, oneBitCount);

        Assert.Equal(expected, actual);
    }
}
