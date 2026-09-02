using DSAExperimentation.LeetCode.RemoveDuplicateLetters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicateLetters;

// Harness only: both arms are RemoveDuplicateLettersSolution's, the same
// methods RemoveDuplicateLettersBenchmarks measures.
public sealed class RemoveDuplicateLettersTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "bcabc", "abc" },
            { "cbacdcbc", "acdb" },
            { "abc", "abc" },
            { "a", "a" },
            { "aaaa", "a" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestSubsequenceByRecursiveSplit_LeetCodeExamples_ReturnsSmallestLexicalResult(
        string s, string expected) =>
        Assert.Equal(expected, RemoveDuplicateLettersSolution.SmallestSubsequenceByRecursiveSplit(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestSubsequenceByStackAndSet_LeetCodeExamples_ReturnsSmallestLexicalResult(
        string s, string expected) =>
        Assert.Equal(expected, RemoveDuplicateLettersSolution.SmallestSubsequenceByStackAndSet(s));
}
