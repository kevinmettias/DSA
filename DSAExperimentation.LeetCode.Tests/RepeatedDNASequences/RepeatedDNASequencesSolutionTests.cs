using DSAExperimentation.LeetCode.RepeatedDNASequences;

namespace DSAExperimentation.LeetCode.Tests.RepeatedDNASequences;

// Harness only: both strategies live in RepeatedDNASequencesSolution - this file
// just pins them to LeetCode's published examples plus the edge cases (too short
// to hold a window, and a window-length string with no repeat) the original test
// never exercised.
public sealed partial class RepeatedDNASequencesSolutionTests
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
        string sequence, string[] expected) =>
        Assert.Equal(expected, RepeatedDNASequencesSolution.FindByFixedWindowSet(sequence));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindByRollingTwoBitMask_LeetCodeExamples_ReturnsRepeatedWindowsInFirstAppearanceOrder(
        string sequence, string[] expected) =>
        Assert.Equal(expected, RepeatedDNASequencesSolution.FindByRollingTwoBitMask(sequence));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they report the same windows in the same order on every
    // example - not merely that each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRepeatedWindows_AgreeOnEveryExample(string sequence, string[] expected) =>
        Assert.Equal(
            RepeatedDNASequencesSolution.FindByFixedWindowSet(sequence),
            RepeatedDNASequencesSolution.FindByRollingTwoBitMask(sequence));
}
