using DSAExperimentation.LeetCode.RepeatedDNASequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RepeatedDNASequences;

// Harness only: the fixed-window scan lives in RepeatedDNASequencesSolution.
// this file just pins it to LeetCode's published examples plus the edge cases
// (too short to hold a window, and a window-length string with no repeat) the
// original test never exercised.
public sealed class RepeatedDNASequencesTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "AAAAACCCCCAAAAACCCCCCAAAAAGGGTTT", ["AAAAACCCCC", "CCCCCAAAAA"] },
            { "AAAAAAAAAAAAA", ["AAAAAAAAAA"] },
            { "ACGTACGTAC", [] },
            { "ACGTACGT", [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindByFixedWindowSet_LeetCodeExamples_ReturnsRepeatedWindowsInFirstAppearanceOrder(
        string s, string[] expected) =>
        Assert.Equal(expected, RepeatedDNASequencesSolution.FindByFixedWindowSet(s));
}
