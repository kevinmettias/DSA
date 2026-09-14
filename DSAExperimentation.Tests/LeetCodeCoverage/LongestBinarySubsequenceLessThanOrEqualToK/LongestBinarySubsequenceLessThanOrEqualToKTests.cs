using DSAExperimentation.LeetCode.LongestBinarySubsequenceLessThanOrEqualToK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestBinarySubsequenceLessThanOrEqualToK;

// Harness only. Both strategies are LongestBinarySubsequenceLessThanOrEqualToKSolution's,
// and pinning them to the same examples is what finally puts the exhaustive subset
// enumeration - previously a benchmark arm asserted by nothing - under test beside
// the greedy scan it is measured against. Every string here is short enough that the
// baseline's 2^n walk stays trivial.
public sealed class LongestBinarySubsequenceLessThanOrEqualToKTests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            // LeetCode example 1: "1001010" with k = 5, whose best subsequence is
            // "00010" - five characters worth 2.
            { "1001010", 5, 5 },

            // LeetCode example 2: "00101001" with k = 1, whose best subsequence is
            // "000001" - six characters worth 1.
            { "00101001", 1, 6 },

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
    [MemberData(nameof(Examples))]
    public void LongestSubsequenceBySubsetEnumeration_LeetCodeExamples_ReturnsLongestAffordableLength(
        string bits, int maxValue, int expected) =>
        Assert.Equal(
            expected,
            LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceBySubsetEnumeration(bits, maxValue));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestSubsequenceByGreedyScan_LeetCodeExamples_ReturnsLongestAffordableLength(
        string bits, int maxValue, int expected) =>
        Assert.Equal(
            expected,
            LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceByGreedyScan(bits, maxValue));
}
