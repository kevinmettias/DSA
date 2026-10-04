using DSAExperimentation.LeetCode.PartitionLabels;

namespace DSAExperimentation.LeetCode.Tests.PartitionLabels;

// Harness only: both strategies live in PartitionLabelsSolution and are asserted
// against the same examples.
public sealed partial class PartitionLabelsSolutionTests
{
    public static TheoryData<string, List<int>> Examples =>
        new()
        {
            // LeetCode examples 1 and 2.
            { "ababcbacadefegdehijhklij", [9, 7, 8] },
            { "eccbbbbdec", [10] },

            // Three distinct letters close a part each; in "abab" both letters recur at
            // the end, so the only part is the whole string.
            { "abc", [1, 1, 1] },
            { "abab", [4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionLabelSizesByBruteForceRescan_LeetCodeExamples_ReturnsExpectedPartitionSizes(
        string text, List<int> expected) =>
        Assert.Equal(expected, PartitionLabelsSolution.PartitionLabelSizesByBruteForceRescan(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionLabelSizesByHashMapOnePass_LeetCodeExamples_ReturnsExpectedPartitionSizes(
        string text, List<int> expected) =>
        Assert.Equal(expected, PartitionLabelsSolution.PartitionLabelSizesByHashMapOnePass(text));
}
