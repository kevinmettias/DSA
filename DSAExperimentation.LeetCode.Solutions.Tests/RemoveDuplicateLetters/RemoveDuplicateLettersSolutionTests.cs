using DSAExperimentation.LeetCode.Solutions.RemoveDuplicateLetters;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveDuplicateLetters;

// Harness only: both arms are RemoveDuplicateLettersSolution's, the same
// methods RemoveDuplicateLettersBenchmarks measures.
public sealed partial class RemoveDuplicateLettersSolutionTests
{
    public static TheoryData<SmallestSubsequenceCase> Cases =>
        new()
        {
            { new SmallestSubsequenceCase(S: "abc", Expected: "abc") },
            { new SmallestSubsequenceCase(S: "aaaa", Expected: "a") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestSubsequenceByRecursiveSplit_Cases_ReturnsSmallestLexicalResult(
        SmallestSubsequenceCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            RemoveDuplicateLettersSolution.SmallestSubsequenceByRecursiveSplit(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestSubsequenceByStackAndSet_Cases_ReturnsSmallestLexicalResult(
        SmallestSubsequenceCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            RemoveDuplicateLettersSolution.SmallestSubsequenceByStackAndSet(testCase.S));

    // One case: the input string and the smallest subsequence it yields.
    // Both values are strings, so each is named at every construction site and a row
    // reads as the case it is rather than as two positions a caller has to keep in
    // order. Nested because it is only ever used inside this test class - it is this
    // harness's own vocabulary, not a type another file would import.
    public readonly record struct SmallestSubsequenceCase(string S, string Expected);
}
