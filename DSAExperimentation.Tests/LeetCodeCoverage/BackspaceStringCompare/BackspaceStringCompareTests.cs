using DSAExperimentation.LeetCode.BackspaceStringCompare;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BackspaceStringCompare;

// Harness only. Both replay strategies are BackspaceStringCompareSolution's - this
// file just pins them to LeetCode's published examples, including the cases that
// decide whether a backspace on already-empty text is the no-op LeetCode says it
// is and whether two strings that agree character-for-character after replay but
// not before are still reported equal.
public sealed class BackspaceStringCompareTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "ab#c", "ad#c", true },
            { "ab##", "c#d#", true },
            { "a#c", "b", false },
            { "bxj##tw", "bxj###tw", false },
            { "y#fo##f", "y#f#o##f", true },
            { "###a", "a", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BackspaceCompareByBclStack_LeetCodeExamples_ReturnsWhetherTypedTextMatches(
        string s, string t, bool expected) =>
        Assert.Equal(expected, BackspaceStringCompareSolution.BackspaceCompareByBclStack(s, t));

    [Theory]
    [MemberData(nameof(Examples))]
    public void BackspaceCompareByStackReplay_LeetCodeExamples_ReturnsWhetherTypedTextMatches(
        string s, string t, bool expected) =>
        Assert.Equal(expected, BackspaceStringCompareSolution.BackspaceCompareByStackReplay(s, t));
}
