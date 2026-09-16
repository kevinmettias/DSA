using DSAExperimentation.LeetCode.BackspaceStringCompare;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BackspaceStringCompare;

// Harness only. Both replay strategies are BackspaceStringCompareSolution's - this
// file just pins them to LeetCode's published examples, including the cases that
// decide whether a backspace on already-empty text is the no-op LeetCode says it
// is and whether two strings that agree character-for-character after replay but
// not before are still reported equal.
public sealed class BackspaceStringCompareTests
{
    public static TheoryData<BackspacePairCase> Examples =>
        new()
        {
            { new BackspacePairCase(S: "ab#c", T: "ad#c", Expected: true) },
            { new BackspacePairCase(S: "ab##", T: "c#d#", Expected: true) },
            { new BackspacePairCase(S: "a#c", T: "b", Expected: false) },
            { new BackspacePairCase(S: "bxj##tw", T: "bxj###tw", Expected: false) },
            { new BackspacePairCase(S: "y#fo##f", T: "y#f#o##f", Expected: true) },
            { new BackspacePairCase(S: "###a", T: "a", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BackspaceCompareByBclStack_LeetCodeExamples_ReturnsWhetherTypedTextMatches(
        BackspacePairCase example)
    {
        var actual = BackspaceStringCompareSolution.BackspaceCompareByBclStack(example.S, example.T);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void BackspaceCompareByStackReplay_LeetCodeExamples_ReturnsWhetherTypedTextMatches(
        BackspacePairCase example)
    {
        var actual = BackspaceStringCompareSolution.BackspaceCompareByStackReplay(example.S, example.T);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two strings the backspace key is applied to, and whether
    // the text they type out is the same. The two strings are the same type and the
    // comparison is not symmetric, so the row names which is which rather than leaving
    // two interchangeable positions. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct BackspacePairCase(string S, string T, bool Expected);
}
