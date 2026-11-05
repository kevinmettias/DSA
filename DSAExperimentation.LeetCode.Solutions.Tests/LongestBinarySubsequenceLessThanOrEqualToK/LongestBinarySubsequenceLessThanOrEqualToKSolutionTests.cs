using DSAExperimentation.LeetCode.Solutions.LongestBinarySubsequenceLessThanOrEqualToK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestBinarySubsequenceLessThanOrEqualToK;

// Harness only. Both strategies are LongestBinarySubsequenceLessThanOrEqualToKSolution's,
// and pinning them to the same cases is what finally puts the exhaustive subset
// enumeration - previously a benchmark arm asserted by nothing - under test beside
// the greedy scan it is measured against. Every string here is short enough that the
// baseline's 2^n walk stays trivial.
public sealed partial class LongestBinarySubsequenceLessThanOrEqualToKSolutionTests
{
    public static TheoryData<string, int, int> Cases =>
        new()
        {
            // Both '1's would cost 5 together, past k = 3, so only the cheaper
            // rightmost one joins the single free '0'.
            { "1101", 3, 2 },

            // A '0' is free no matter what k is.
            { "0", 5, 1 },

            // A lone '1' is worth 1, which k = 0 cannot afford, leaving nothing.
            { "1", 0, 0 },

            // Zeros are always all takeable, so the whole string qualifies.
            { "0000", 0, 4 },

            // k admits the entire value, so nothing is dropped.
            { "1111", 15, 4 },

            // k = 0 admits only zeros - the single '0' here, and neither '1'.
            { "110", 0, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestSubsequenceBySubsetEnumeration_Cases_ReturnsLongestAffordableLength(
        string bits, int maxValue, int expected)
    {
        var actual = LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceBySubsetEnumeration(
            bits, maxValue);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestSubsequenceByGreedyScan_Cases_ReturnsLongestAffordableLength(
        string bits, int maxValue, int expected)
    {
        var actual = LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceByGreedyScan(bits, maxValue);

        Assert.Equal(expected, actual);
    }
}
