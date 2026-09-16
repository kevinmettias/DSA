using DSAExperimentation.LeetCode.GreatestCommonDivisorOfStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GreatestCommonDivisorOfStrings;

// Harness only. Both strategies are GreatestCommonDivisorOfStringsSolution's - the
// str1+str2 == str2+str1 concatenation-equality baseline and this repo's
// PrefixFunctionSearch period read - pinned here to LeetCode's published examples plus
// two identical strings and a prefix-sharing pair that has no common divisor at all.
public sealed partial class GreatestCommonDivisorOfStringsTests
{
    public static TheoryData<GcdExample> Examples =>
        new()
        {
            { new GcdExample(Str1: "ABCABC", Str2: "ABC", Expected: "ABC") },
            { new GcdExample(Str1: "ABABAB", Str2: "ABAB", Expected: "AB") },
            { new GcdExample(Str1: "LEET", Str2: "CODE", Expected: "") },
            { new GcdExample(Str1: "ABCABC", Str2: "ABCABC", Expected: "ABCABC") },
            { new GcdExample(Str1: "ABCDEF", Str2: "ABC", Expected: "") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GcdOfStringsByConcatenationEquality_LeetCodeExamples_ReturnsSharedDivisor(GcdExample example)
    {
        var actual = GreatestCommonDivisorOfStringsSolution.GcdOfStringsByConcatenationEquality(
            example.Str1, example.Str2);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GcdOfStringsByPrefixFunctionPeriod_LeetCodeExamples_ReturnsSharedDivisor(GcdExample example)
    {
        var actual = GreatestCommonDivisorOfStringsSolution.GcdOfStringsByPrefixFunctionPeriod(
            example.Str1, example.Str2);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two strings the divisor is shared between, and the
    // divisor itself. The pair is a symmetric question - the solution's own note says
    // so - but `Expected` is the answer rather than a third input, and as three
    // adjacent `string` positions a caller could hand the answer over in an input slot
    // with the compiler none the wiser.
    public readonly record struct GcdExample(string Str1, string Str2, string Expected);
}
