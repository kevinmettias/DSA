using DSAExperimentation.LeetCode.LongestCommonPrefix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonPrefix;

// Harness only. Both strategies are LongestCommonPrefixSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class LongestCommonPrefixTests
{
    public static TheoryData<string[], string> Examples =>
        new()
        {
            { ["flower", "flow", "flight"], "fl" },
            { ["dog", "racecar", "car"], "" },
            { ["interspecies", "interstellar", "interstate"], "inters" },
            { [""], "" },
            { ["single"], "single" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PrefixByLinearScan_LeetCodeExamples_ReturnsSharedPrefix(string[] values, string expected) =>
        Assert.Equal(expected, LongestCommonPrefixSolution.PrefixByLinearScan(values));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PrefixByBinarySearch_LeetCodeExamples_ReturnsSharedPrefix(string[] values, string expected) =>
        Assert.Equal(expected, LongestCommonPrefixSolution.PrefixByBinarySearch(values));
}
