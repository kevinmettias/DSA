using DSAExperimentation.LeetCode.PartitionLabels;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionLabels;

// Harness only: both strategies live in PartitionLabelsSolution and are asserted
// against the same examples.
public sealed class PartitionLabelsTests
{
    public static TheoryData<string, List<int>> Examples =>
        new()
        {
            { "ababcbacadefegdehijhklij", [9, 7, 8] },
            { "abc", [1, 1, 1] },
            { "abab", [4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionLabelSizesByBruteForceRescan_LeetCodeExamples_ReturnsExpectedPartitionSizes(
        string s, List<int> expected) =>
        Assert.Equal(expected, PartitionLabelsSolution.PartitionLabelSizesByBruteForceRescan(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionLabelSizesByHashMapOnePass_LeetCodeExamples_ReturnsExpectedPartitionSizes(
        string s, List<int> expected) =>
        Assert.Equal(expected, PartitionLabelsSolution.PartitionLabelSizesByHashMapOnePass(s));
}
