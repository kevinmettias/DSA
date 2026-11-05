using DSAExperimentation.LeetCode.Solutions.LongestCommonPrefix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestCommonPrefix;

// Harness only. Both strategies are LongestCommonPrefixSolution's - this file
// just pins them to a six-letter prefix shared by three words and to a lone empty
// string. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class LongestCommonPrefixSolutionTests
{
    public static TheoryData<string[], string> Cases =>
        new()
        {
            { ["interspecies", "interstellar", "interstate"], "inters" },
            { [""], "" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PrefixByLinearScan_Cases_ReturnsSharedPrefix(string[] values, string expected) =>
        Assert.Equal(expected, LongestCommonPrefixSolution.PrefixByLinearScan(values));

    [Theory]
    [MemberData(nameof(Cases))]
    public void PrefixByBinarySearch_Cases_ReturnsSharedPrefix(string[] values, string expected) =>
        Assert.Equal(expected, LongestCommonPrefixSolution.PrefixByBinarySearch(values));
}
