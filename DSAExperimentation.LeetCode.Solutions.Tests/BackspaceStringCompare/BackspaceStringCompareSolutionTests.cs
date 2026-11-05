using DSAExperimentation.LeetCode.Solutions.BackspaceStringCompare;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BackspaceStringCompare;

// Harness only. Both replay strategies are BackspaceStringCompareSolution's - this
// file pins them to two cases: a backspace on already-empty text, which must be the
// no-op LeetCode says it is, and two strings one backspace apart, where the extra
// backspace erases one more character and the typed text no longer matches.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class BackspaceStringCompareSolutionTests
{
    public static TheoryData<BackspacePairCase> Cases =>
        new()
        {
            { new BackspacePairCase(S: "bxj##tw", T: "bxj###tw", Expected: false) },
            { new BackspacePairCase(S: "###a", T: "a", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsTypedTextEqualByBclStack_Cases_ReturnsWhetherTypedTextMatches(
        BackspacePairCase testCase)
    {
        var actual = BackspaceStringCompareSolution.IsTypedTextEqualByBclStack(testCase.S, testCase.T);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsTypedTextEqualByStackReplay_Cases_ReturnsWhetherTypedTextMatches(
        BackspacePairCase testCase)
    {
        var actual = BackspaceStringCompareSolution.IsTypedTextEqualByStackReplay(testCase.S, testCase.T);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the two strings the backspace key is applied to, and whether
    // the text they type out is the same. The two strings are the same type and the
    // comparison is not symmetric, so the row names which is which rather than leaving
    // two interchangeable positions. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct BackspacePairCase(string S, string T, bool Expected);
}
