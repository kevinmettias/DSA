using DSAExperimentation.LeetCode.Solutions.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

// Harness only. Both strategies are
// SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution's - this file pins them
// to the boundary cases the greedy has to get
// right. The window-rescan baseline (previously untested scaffolding inlined in the
// benchmark) is asserted here for the first time.
public sealed partial class SmallestKLengthSubsequenceWithOccurrencesOfALetterSolutionTests
{
    public static TheoryData<SubsequenceCase> Cases =>
        new()
        {
            // k equals the whole string, so the answer is s itself even though it is
            // strictly decreasing and the greedy would love to pop every character.
            new SubsequenceCase(S: "dcba", K: 4, Letter: 'a', Repetition: 1, Expected: "dcba"),

            // The single character taken must be the letter, so the smaller 'a' ahead
            // of it cannot win the slot.
            new SubsequenceCase(S: "ba", K: 1, Letter: 'b', Repetition: 1, Expected: "b"),

            // The mirror case: 'b' comes first and is larger, but popping it is
            // allowed because the required 'a' is still reachable.
            new SubsequenceCase(S: "ba", K: 1, Letter: 'a', Repetition: 1, Expected: "a"),

            // Popping the leading 'b' would leave no 'b' behind to satisfy the
            // repetition, so it must stay even though 'a' is smaller.
            new SubsequenceCase(S: "baaa", K: 2, Letter: 'b', Repetition: 1, Expected: "ba"),

            // Repetition is already satisfiable from the tail, so the greedy is free
            // to take the two leading 'a's before the required 'b'.
            new SubsequenceCase(S: "aabbaa", K: 3, Letter: 'b', Repetition: 1, Expected: "aab"),

            // Every 'b' but the last two must be dropped to make room for the 'a's,
            // while repetition keeps two of them.
            new SubsequenceCase(S: "aaabbb", K: 3, Letter: 'b', Repetition: 2, Expected: "abb"),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestSubsequenceByWindowRescan_Cases_ReturnsSmallestFeasibleSubsequence(
        SubsequenceCase testCase)
    {
        var actual = SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution.SmallestSubsequenceByWindowRescan(
            testCase.S, testCase.K, testCase.Letter, testCase.Repetition);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestSubsequenceByMonotonicStack_Cases_ReturnsSmallestFeasibleSubsequence(
        SubsequenceCase testCase)
    {
        var actual = SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution.SmallestSubsequenceByMonotonicStack(
            testCase.S, testCase.K, testCase.Letter, testCase.Repetition);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the source string, the subsequence length, the letter
    // that must occur in it, how many times that letter must occur, and the smallest
    // feasible subsequence. The five are one case, so the signature carries one
    // parameter rather than five positions.
    public readonly record struct SubsequenceCase(
        string S,
        int K,
        char Letter,
        int Repetition,
        string Expected);
}
