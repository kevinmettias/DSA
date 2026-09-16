using DSAExperimentation.LeetCode.TheKthFactorOfN;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheKthFactorOfN;

// Harness only. Both strategies live in TheKthFactorOfNSolution - this file pins them
// to LeetCode's published examples plus the shapes a sqrt-anchored divisor walk has to
// get right: a perfect square (whose root must be counted exactly once), a prime, the
// largest divisor of all (`number` itself), and a `rank` that runs past the end of the
// divisor list.
public sealed partial class TheKthFactorOfNTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            // LC example 1: divisors of 12 are 1, 2, 3, 4, 6, 12.
            { 12, 3, 3 },

            // LC example 2: a prime has only 1 and itself.
            { 7, 2, 7 },

            // LC example 3: 4 has three divisors, so there is no fourth.
            { 4, 4, -1 },

            // The smallest input: 1 is its own only divisor.
            { 1, 1, 1 },

            // A prime asked for one factor too many.
            { 7, 3, -1 },

            // A perfect square: divisors of 16 are 1, 2, 4, 8, 16 - the root 4 sits on
            // the anchor and must be counted once, not twice.
            { 16, 3, 4 },
            { 16, 5, 16 },
            { 16, 6, -1 },

            // The last divisor is always n itself.
            { 12, 6, 12 },

            // A highly composite value at the top of LC's range: divisors of 1000 are
            // 1, 2, 4, 5, 8, 10, 20, 25, 40, 50, 100, 125, 200, 250, 500, 1000.
            { 1000, 1, 1 },
            { 1000, 9, 40 },
            { 1000, 16, 1000 },
            { 1000, 17, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthFactorByFullRangeScan_LeetCodeExamples_ReturnsExpectedFactorOrMinusOne(
        int number, int rank, int expected)
    {
        var actual = TheKthFactorOfNSolution.KthFactorByFullRangeScan(number, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthFactorByBinarySearchAnchor_LeetCodeExamples_ReturnsExpectedFactorOrMinusOne(
        int number, int rank, int expected)
    {
        var actual = TheKthFactorOfNSolution.KthFactorByBinarySearchAnchor(number, rank);

        Assert.Equal(expected, actual);
    }
}
