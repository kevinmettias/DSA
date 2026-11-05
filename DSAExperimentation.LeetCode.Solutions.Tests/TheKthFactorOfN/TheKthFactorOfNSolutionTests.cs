using DSAExperimentation.LeetCode.Solutions.TheKthFactorOfN;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TheKthFactorOfN;

// Harness only. Both strategies live in TheKthFactorOfNSolution - this file pins them
// to the shapes a sqrt-anchored divisor walk has to
// get right: a perfect square (whose root must be counted exactly once), a prime, the
// largest divisor of all (`number` itself), and a `rank` that runs past the end of the
// divisor list.
public sealed partial class TheKthFactorOfNSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            // A prime asked for one factor too many.
            { 7, 3, -1 },

            // A perfect square: divisors of 16 are 1, 2, 4, 8, 16 - the root 4 sits on
            // the anchor and must be counted once, not twice.
            { 16, 3, 4 },
            { 16, 6, -1 },

            // The last divisor is always n itself.
            { 12, 6, 12 },

            // A highly composite value at the top of LC's range: divisors of 1000 are
            // 1, 2, 4, 5, 8, 10, 20, 25, 40, 50, 100, 125, 200, 250, 500, 1000.
            { 1000, 9, 40 },
            { 1000, 17, -1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void KthFactorByFullRangeScan_Cases_ReturnsExpectedFactorOrMinusOne(
        int number, int rank, int expected)
    {
        var actual = TheKthFactorOfNSolution.KthFactorByFullRangeScan(number, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void KthFactorByBinarySearchAnchor_Cases_ReturnsExpectedFactorOrMinusOne(
        int number, int rank, int expected)
    {
        var actual = TheKthFactorOfNSolution.KthFactorByBinarySearchAnchor(number, rank);

        Assert.Equal(expected, actual);
    }
}
