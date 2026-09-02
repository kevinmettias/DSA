using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfSwapsToMakeTheStringBalanced;

// LeetCode 1963. Minimum Number of Swaps to Make the String Balanced: this repo's own
// Stack<char> tracks unmatched '[' the same way ValidParenthesesTests does. Every ']'
// pops a pending '[' when one is available; when the stack is empty, that ']' has no
// partner anywhere to its left, so a swap is counted and a virtual '[' is pushed in its
// place, standing in for whichever later ']' actually gets swapped with it. The final
// swap count is provably optimal (equivalent to ceil(maxDeficit / 2), the standard
// result for this problem) because every unmatched closer this scan finds is genuinely
// unresolvable without pulling in an opener from beyond the current position.
public sealed partial class MinimumNumberOfSwapsToMakeTheStringBalancedTests
{
    [Theory]
    [InlineData("][][", 1)]
    [InlineData("]]][[[", 2)]
    [InlineData("[]", 0)]
    public void MinSwaps_LeetCodeExamples_ReturnsMinimumSwapCount(string s, int expected)
        => Assert.Equal(expected, MinSwaps(s));

    private static int MinSwaps(string s)
    {
        var open = new RepoCharStack();
        var swaps = 0;

        foreach (var ch in s)
        {
            if (ch == '[')
            {
                open.Push(ch);
                continue;
            }

            if (open.TryPop(out _))
            {
                continue;
            }

            swaps++;
            open.Push('[');
        }

        return swaps;
    }
}
