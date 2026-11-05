using DSAExperimentation.LeetCode.Solutions.GreatestCommonDivisorOfStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GreatestCommonDivisorOfStrings;

// Harness only. Both strategies are GreatestCommonDivisorOfStringsSolution's - the
// str1+str2 == str2+str1 concatenation-equality baseline and this repo's
// PrefixFunctionSearch period read - pinned here to two identical strings. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class GreatestCommonDivisorOfStringsSolutionTests
{
    public static TheoryData<GcdCase> Cases =>
        new()
        {
            // A string divides itself, so two equal strings answer the whole string.
            { new GcdCase(Str1: "ABCABC", Str2: "ABCABC", Expected: "ABCABC") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GcdOfStringsByConcatenationEquality_Cases_ReturnsSharedDivisor(GcdCase testCase)
    {
        var actual = GreatestCommonDivisorOfStringsSolution.GcdOfStringsByConcatenationEquality(
            testCase.Str1, testCase.Str2);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void GcdOfStringsByPrefixFunctionPeriod_Cases_ReturnsSharedDivisor(GcdCase testCase)
    {
        var actual = GreatestCommonDivisorOfStringsSolution.GcdOfStringsByPrefixFunctionPeriod(
            testCase.Str1, testCase.Str2);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the two strings the divisor is shared between, and the
    // divisor itself. The pair is a symmetric question - the solution's own note says
    // so - but `Expected` is the answer rather than a third input, and as three
    // adjacent `string` positions a caller could hand the answer over in an input slot
    // with the compiler none the wiser.
    public readonly record struct GcdCase(string Str1, string Str2, string Expected);
}
