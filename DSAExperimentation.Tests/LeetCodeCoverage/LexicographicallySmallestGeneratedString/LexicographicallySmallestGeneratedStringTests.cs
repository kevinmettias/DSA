using DSAExperimentation.LeetCode.LexicographicallySmallestGeneratedString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallySmallestGeneratedString;

// Harness only. Both fill strategies are
// LexicographicallySmallestGeneratedStringSolution's - this file just pins
// them to LeetCode's published examples plus hand-verified cases covering
// every branch the published examples miss: a consistent overlapping-'T'
// merge ("TT"/"aa"), an 'F' window that needs its rightmost free character
// bumped ("TF"/"a"), and an 'F' window whose every position is pinned by
// surrounding 'T's, so it cannot be broken at all ("TFTF"/"aaa").
public sealed class LexicographicallySmallestGeneratedStringTests
{
    public static TheoryData<string, string, string> Examples =>
        new()
        {
            { "TFTF", "ab", "ababa" },
            { "TFTF", "abc", "" },
            { "F", "d", "a" },
            { "TT", "aa", "aaa" },
            { "TF", "a", "ab" },
            { "TFTF", "aaa", "" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateStringByDirectFill_LeetCodeExamples_ReturnsSmallestGeneratedString(
        string str1, string str2, string expected) =>
        Assert.Equal(expected, LexicographicallySmallestGeneratedStringSolution.GenerateStringByDirectFill(str1, str2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateStringByZFunctionConsistency_LeetCodeExamples_ReturnsSmallestGeneratedString(
        string str1, string str2, string expected) =>
        Assert.Equal(
            expected, LexicographicallySmallestGeneratedStringSolution.GenerateStringByZFunctionConsistency(str1, str2));
}
