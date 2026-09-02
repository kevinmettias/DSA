using DSAExperimentation.LeetCode.PartitionString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionString;

// Harness only. Both strategies are PartitionStringSolution's - this file
// just pins them to LeetCode's published examples, including the trailing-
// duplicate-segment case ("aaaa") that the leftover segment must be dropped
// from, not force-added.
public sealed class PartitionStringTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "abbccccd", ["a", "b", "bc", "c", "cc", "d"] },
            { "aaaa", ["a", "aa"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionByHashSetScan_LeetCodeExamples_ReturnsUniqueSegments(string s, string[] expected) =>
        Assert.Equal(expected, PartitionStringSolution.PartitionByHashSetScan(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionBySetScan_LeetCodeExamples_ReturnsUniqueSegments(string s, string[] expected) =>
        Assert.Equal(expected, PartitionStringSolution.PartitionBySetScan(s));
}
