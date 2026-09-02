using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

// LeetCode 2030. Smallest K-Length Subsequence With Occurrences of a Letter: the
// same greedy monotonic-stack shape as RemoveDuplicateLettersTests, built on this
// repo's own Stack<char>, but popping is gated on two extra counts beyond "is the
// top bigger" - enough characters must still remain ahead to reach length k, and
// popping a `letter` occurrence must not drop the remaining supply below
// `repetition`.
public sealed class SmallestKLengthSubsequenceWithOccurrencesOfALetterTests
{
    [Theory]
    [InlineData("leet", new[] { 3, 1 }, 'e', "eet")]
    [InlineData("leetcode", new[] { 4, 2 }, 'e', "ecde")]
    [InlineData("bb", new[] { 2, 2 }, 'b', "bb")]
    public void SmallestSubsequence_LeetCodeExamples_ReturnsExpected(
        string s, int[] kAndRepetition, char letter, string expected)
    {
        var actual = SmallestSubsequence(s, kAndRepetition[0], letter, kAndRepetition[1]);

        Assert.Equal(expected, actual);
    }

    private static string SmallestSubsequence(string s, int k, char letter, int repetition)
    {
        var n = s.Length;
        var letterSuffixCount = BuildLetterSuffixCount(s, letter);

        var stack = new RepoCharStack();
        var lettersInStack = 0;
        var context = new SweepContext(s, n, k, letter, repetition, letterSuffixCount);

        for (var i = 0; i < n; i++)
        {
            ProcessCharacter(context, stack, ref lettersInStack, i);
        }

        return DrainStack(stack);
    }

    private static int[] BuildLetterSuffixCount(string s, char letter)
    {
        var letterSuffixCount = new int[s.Length + 1];
        for (var i = s.Length - 1; i >= 0; i--)
        {
            letterSuffixCount[i] = letterSuffixCount[i + 1] + (s[i] == letter ? 1 : 0);
        }

        return letterSuffixCount;
    }

    private static string DrainStack(RepoCharStack stack)
    {
        var result = new char[stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return new string(result);
    }

    private static void ProcessCharacter(SweepContext context, RepoCharStack stack, ref int lettersInStack, int index)
    {
        ShrinkWhileNotFeasible(context, stack, ref lettersInStack, index);

        if (stack.Count >= context.K)
        {
            return;
        }

        TryPushCharacter(context, stack, ref lettersInStack, context.S[index]);
    }

    private static void ShrinkWhileNotFeasible(SweepContext context, RepoCharStack stack, ref int lettersInStack, int index)
    {
        var c = context.S[index];

        while (stack.TryPeek(out var top)
               && top > c
               && stack.Count + (context.N - index) > context.K
               && (top != context.Letter || lettersInStack - 1 + context.LetterSuffixCount[index] >= context.Repetition))
        {
            stack.TryPop(out _);
            if (top == context.Letter)
            {
                lettersInStack--;
            }
        }
    }

    private static void TryPushCharacter(SweepContext context, RepoCharStack stack, ref int lettersInStack, char c)
    {
        if (c == context.Letter)
        {
            stack.Push(c);
            lettersInStack++;
        }
        else if (context.K - stack.Count > context.Repetition - lettersInStack)
        {
            stack.Push(c);
        }
    }

    private readonly record struct SweepContext(
        string S, int N, int K, char Letter, int Repetition, int[] LetterSuffixCount);
}
