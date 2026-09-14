using DSAExperimentation.LeetCode.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

// Harness only. Both strategies are
// SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution's - this file pins them
// to LeetCode's published examples plus the boundary cases the greedy has to get
// right. The window-rescan baseline (previously untested scaffolding inlined in the
// benchmark) is asserted here for the first time.
public sealed class SmallestKLengthSubsequenceWithOccurrencesOfALetterTests
{
    public static TheoryData<string, int, char, int, string> Examples =>
        new()
        {
            // LeetCode's three published examples.
            { "leet", 3, 'e', 1, "eet" },
            { "leetcode", 4, 'e', 2, "ecde" },
            { "bb", 2, 'b', 2, "bb" },

            // k equals the whole string, so the answer is s itself even though it is
            // strictly decreasing and the greedy would love to pop every character.
            { "dcba", 4, 'a', 1, "dcba" },

            // The single character taken must be the letter, so the smaller 'a' ahead
            // of it cannot win the slot.
            { "ba", 1, 'b', 1, "b" },

            // The mirror case: 'b' comes first and is larger, but popping it is
            // allowed because the required 'a' is still reachable.
            { "ba", 1, 'a', 1, "a" },

            // Popping the leading 'b' would leave no 'b' behind to satisfy the
            // repetition, so it must stay even though 'a' is smaller.
            { "baaa", 2, 'b', 1, "ba" },

            // Repetition is already satisfiable from the tail, so the greedy is free
            // to take the two leading 'a's before the required 'b'.
            { "aabbaa", 3, 'b', 1, "aab" },

            // Every 'b' but the last two must be dropped to make room for the 'a's,
            // while repetition keeps two of them.
            { "aaabbb", 3, 'b', 2, "abb" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestSubsequenceByWindowRescan_LeetCodeExamples_ReturnsSmallestFeasibleSubsequence(
        string s, int k, char letter, int repetition, string expected) =>
        Assert.Equal(
            expected,
            SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution.SmallestSubsequenceByWindowRescan(
                s, k, letter, repetition));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestSubsequenceByMonotonicStack_LeetCodeExamples_ReturnsSmallestFeasibleSubsequence(
        string s, int k, char letter, int repetition, string expected) =>
        Assert.Equal(
            expected,
            SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution.SmallestSubsequenceByMonotonicStack(
                s, k, letter, repetition));
}
