using DSAExperimentation.LeetCode.Solutions.RepeatedDNASequences;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RepeatedDNASequences;

// Harness only: both strategies live in RepeatedDNASequencesSolution - this file
// just pins them to the edge cases (too short
// to hold a window, and a window-length string with no repeat) the original test
// never exercised.
public sealed partial class RepeatedDNASequencesSolutionTests
{
    public static TheoryData<string, string[]> Cases =>
        new()
        {
            { "ACGTACGTAC", [] },
            { "ACGTACGT", [] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindByFixedWindowSet_Cases_ReturnsRepeatedWindowsInFirstAppearanceOrder(
        string sequence, string[] expected) =>
        Assert.Equal(expected, RepeatedDNASequencesSolution.FindByFixedWindowSet(sequence));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindByRollingTwoBitMask_Cases_ReturnsRepeatedWindowsInFirstAppearanceOrder(
        string sequence, string[] expected) =>
        Assert.Equal(expected, RepeatedDNASequencesSolution.FindByRollingTwoBitMask(sequence));
}
