using DSAExperimentation.LeetCode.GreatestCommonDivisorOfStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GreatestCommonDivisorOfStrings;

// Harness only. Both strategies are GreatestCommonDivisorOfStringsSolution's - the
// str1+str2 == str2+str1 concatenation-equality baseline and this repo's
// PrefixFunctionSearch period read - pinned here to LeetCode's published examples plus
// two identical strings and a prefix-sharing pair that has no common divisor at all.
public sealed class GreatestCommonDivisorOfStringsTests
{
    public static TheoryData<string, string, string> Examples =>
        new()
        {
            { "ABCABC", "ABC", "ABC" },
            { "ABABAB", "ABAB", "AB" },
            { "LEET", "CODE", "" },
            { "ABCABC", "ABCABC", "ABCABC" },
            { "ABCDEF", "ABC", "" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GcdOfStringsByConcatenationEquality_LeetCodeExamples_ReturnsSharedDivisor(
        string str1, string str2, string expected) =>
        Assert.Equal(expected, GreatestCommonDivisorOfStringsSolution.GcdOfStringsByConcatenationEquality(str1, str2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GcdOfStringsByPrefixFunctionPeriod_LeetCodeExamples_ReturnsSharedDivisor(
        string str1, string str2, string expected) =>
        Assert.Equal(expected, GreatestCommonDivisorOfStringsSolution.GcdOfStringsByPrefixFunctionPeriod(str1, str2));
}
