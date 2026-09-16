using DSAExperimentation.LeetCode.RemoveDuplicateLetters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicateLetters;

// Harness only: both arms are RemoveDuplicateLettersSolution's, the same
// methods RemoveDuplicateLettersBenchmarks measures.
public sealed partial class RemoveDuplicateLettersTests
{
    public static TheoryData<SmallestSubsequenceCase> Examples =>
        new()
        {
            { new SmallestSubsequenceCase(S: "bcabc", Expected: "abc") },
            { new SmallestSubsequenceCase(S: "cbacdcbc", Expected: "acdb") },
            { new SmallestSubsequenceCase(S: "abc", Expected: "abc") },
            { new SmallestSubsequenceCase(S: "a", Expected: "a") },
            { new SmallestSubsequenceCase(S: "aaaa", Expected: "a") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestSubsequenceByRecursiveSplit_LeetCodeExamples_ReturnsSmallestLexicalResult(
        SmallestSubsequenceCase example) =>
        Assert.Equal(
            example.Expected,
            RemoveDuplicateLettersSolution.SmallestSubsequenceByRecursiveSplit(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestSubsequenceByStackAndSet_LeetCodeExamples_ReturnsSmallestLexicalResult(
        SmallestSubsequenceCase example) =>
        Assert.Equal(
            example.Expected,
            RemoveDuplicateLettersSolution.SmallestSubsequenceByStackAndSet(example.S));

    // One LeetCode example: the input string and the smallest subsequence it yields.
    // Both values are strings, so each is named at every construction site and a row
    // reads as the case it is rather than as two positions a caller has to keep in
    // order. Nested because it is only ever used inside this test class - it is this
    // harness's own vocabulary, not a type another file would import.
    public readonly record struct SmallestSubsequenceCase(string S, string Expected);
}
