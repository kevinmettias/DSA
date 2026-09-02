using RepoCharSet = DSAExperimentation.DataStructures.Set.Set<char>;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicateLetters;

// LeetCode 316. Remove Duplicate Letters: the classic greedy monotonic-stack walk,
// built on this repo's own Stack<char> (the candidate answer, popped whenever a
// smaller letter can still replace the top later) and Set<char> (which letters are
// currently on the stack, so a later occurrence of one already placed is skipped).
public sealed partial class RemoveDuplicateLettersTests
{
    [Fact]
    public void RemoveDuplicateLetters_ClassicExample_ReturnsSmallestLexicalResult()
        => Assert.Equal("abc", RemoveDuplicateLetters("bcabc"));

    [Fact]
    public void RemoveDuplicateLetters_SecondExample_ReturnsSmallestLexicalResult()
        => Assert.Equal("acdb", RemoveDuplicateLetters("cbacdcbc"));

    [Fact]
    public void RemoveDuplicateLetters_NoDuplicates_ReturnsInputUnchanged()
        => Assert.Equal("abc", RemoveDuplicateLetters("abc"));

    private static string RemoveDuplicateLetters(string s)
    {
        var lastOccurrence = new int[26];
        for (var i = 0; i < s.Length; i++)
        {
            lastOccurrence[s[i] - 'a'] = i;
        }

        var candidate = new CandidateStack();

        for (var i = 0; i < s.Length; i++)
        {
            AppendCandidateLetter(candidate, lastOccurrence, s[i], i);
        }

        var result = new char[candidate.Stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            candidate.Stack.TryPop(out result[i]);
        }

        return new string(result);
    }

    // Skips a letter already on the candidate stack, otherwise pops any larger
    // letter that still reappears later in the string before pushing this one -
    // the greedy monotonic-stack step that keeps the candidate answer smallest.
    private static void AppendCandidateLetter(CandidateStack candidate, int[] lastOccurrence, char c, int index)
    {
        if (candidate.OnStack.Has(c))
        {
            return;
        }

        while (candidate.Stack.TryPeek(out var top) && top > c && lastOccurrence[top - 'a'] > index)
        {
            candidate.Stack.TryPop(out _);
            candidate.OnStack.TryRemove(top);
        }

        candidate.Stack.Push(c);
        candidate.OnStack.TryAdd(c);
    }

    // The candidate answer (as a stack) and which letters are currently on it,
    // always mutated together.
    private sealed class CandidateStack
    {
        public readonly RepoCharStack Stack = new();
        public readonly RepoCharSet OnStack = new();
    }
}
