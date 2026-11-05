using DSAExperimentation.LeetCode.Solutions.PartitionLabels;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PartitionLabels;

// Harness only: both strategies live in PartitionLabelsSolution and are asserted
// against the same cases.
public sealed partial class PartitionLabelsSolutionTests
{
    public static TheoryData<string, List<int>> Cases =>
        new()
        {
            // Three distinct letters close a part each; in "abab" both letters recur at
            // the end, so the only part is the whole string.
            { "abc", [1, 1, 1] },
            { "abab", [4] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PartitionLabelSizesByBruteForceRescan_Cases_ReturnsExpectedPartitionSizes(
        string text, List<int> expected) =>
        Assert.Equal(expected, PartitionLabelsSolution.PartitionLabelSizesByBruteForceRescan(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void PartitionLabelSizesByHashMapOnePass_Cases_ReturnsExpectedPartitionSizes(
        string text, List<int> expected) =>
        Assert.Equal(expected, PartitionLabelsSolution.PartitionLabelSizesByHashMapOnePass(text));
}
