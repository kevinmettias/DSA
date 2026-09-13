using DSAExperimentation.LeetCode.ClosestDivisors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestDivisors;

// Harness only. Both strategies are ClosestDivisorsSolution's - the sqrt-anchored
// binary search and the full-range divisor scan that used to live untested in the
// benchmark - pinned to LeetCode's published examples plus the smallest inputs and a
// case where the tighter pair comes from num + 1 rather than num + 2.
public sealed class ClosestDivisorsTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 8, 3, 3 },
            { 123, 5, 25 },
            { 999, 25, 40 },
            { 1, 1, 2 },
            { 2, 2, 2 },
            { 99, 10, 10 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestPairByBinarySearchAnchor_LeetCodeExamples_ReturnsClosestPair(
        int num, int expectedFirst, int expectedSecond) =>
        Assert.Equal((expectedFirst, expectedSecond), ClosestDivisorsSolution.ClosestPairByBinarySearchAnchor(num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestPairByDivisorScan_LeetCodeExamples_ReturnsClosestPair(
        int num, int expectedFirst, int expectedSecond) =>
        Assert.Equal((expectedFirst, expectedSecond), ClosestDivisorsSolution.ClosestPairByDivisorScan(num));
}
