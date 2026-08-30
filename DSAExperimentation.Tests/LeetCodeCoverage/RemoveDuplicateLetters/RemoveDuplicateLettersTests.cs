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

        var stack = new RepoCharStack();
        var onStack = new RepoCharSet();

        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];

            if (onStack.Has(c))
            {
                continue;
            }

            while (stack.TryPeek(out var top) && top > c && lastOccurrence[top - 'a'] > i)
            {
                stack.TryPop(out _);
                onStack.TryRemove(top);
            }

            stack.Push(c);
            onStack.TryAdd(c);
        }

        var result = new char[stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return new string(result);
    }
}
