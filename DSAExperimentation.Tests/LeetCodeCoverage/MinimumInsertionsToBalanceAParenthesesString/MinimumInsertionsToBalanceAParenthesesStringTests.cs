using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumInsertionsToBalanceAParenthesesString;

// LeetCode 1541. Minimum Insertions to Balance a Parentheses String: this repo's
// Stack<char> tracks unmatched '(' openers the same way
// MinimumAddToMakeParenthesesValidTests does; the only extra wrinkle is that a
// balanced unit here is '(' matched by TWO consecutive ')', not one, so every ')'
// also looks one character ahead - a ')' with no adjacent partner needs an inserted
// closer, and a ')' with nothing left on the stack needs an inserted opener too.
// Whatever openers remain on the stack at the end each still need two closers.
public sealed partial class MinimumInsertionsToBalanceAParenthesesStringTests
{
    [Fact]
    public void MinInsertions_OneOpenerMissingASecondCloser_ReturnsOne() => Assert.Equal(1, MinInsertions("(()))"));

    [Fact]
    public void MinInsertions_AlreadyBalanced_ReturnsZero() => Assert.Equal(0, MinInsertions("())"));

    [Fact]
    public void MinInsertions_MixedUnmatchedOpenersAndClosers_ReturnsCombinedInsertions()
        => Assert.Equal(3, MinInsertions("))())("));

    [Fact]
    public void MinInsertions_AllOpeners_ReturnsTwoInsertionsPerOpener() => Assert.Equal(12, MinInsertions("(((((("));

    [Fact]
    public void MinInsertions_AllClosers_ReturnsOneInsertionPerUnmatchedPair()
        => Assert.Equal(5, MinInsertions(")))))))"));

    private static int MinInsertions(string s)
    {
        var openers = new RepoCharStack();
        var insertions = 0;
        var i = 0;

        while (i < s.Length)
        {
            if (s[i] == '(')
            {
                openers.Push(s[i]);
                i++;
                continue;
            }

            var hasAdjacentCloser = i + 1 < s.Length && s[i + 1] == ')';
            if (!hasAdjacentCloser)
            {
                insertions++;
            }

            if (!openers.TryPop(out _))
            {
                insertions++;
            }

            i += hasAdjacentCloser ? 2 : 1;
        }

        return insertions + (openers.Count * 2);
    }
}
