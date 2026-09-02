using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNestingDepthOfTwoValidParenthesesStrings;

// LeetCode 1111. Maximum Nesting Depth of Two Valid Parentheses Strings: this
// repo's Stack<char> tracks the currently-open brackets while walking the
// sequence once; each character's group is the current stack depth's parity,
// which splits the odd nesting levels from the even ones between the two
// output subsequences - the same "explicit repo Stack" move
// ValidParenthesesTests itself makes.
public sealed partial class MaximumNestingDepthOfTwoValidParenthesesStringsTests
{
    [Fact]
    public void MaxDepthAfterSplit_NestedParentheses_HalvesTheOriginalMaxDepth()
    {
        const string seq = "(((())))";

        var groups = MaxDepthAfterSplit(seq);
        var group0 = SubsequenceOf(seq, groups, group: 0);
        var group1 = SubsequenceOf(seq, groups, group: 1);
        var group0Depth = MaxDepth(group0);
        var group1Depth = MaxDepth(group1);

        Assert.Equal(MaxDepth(seq), group0Depth + group1Depth);
        Assert.True(group0Depth <= 2);
        Assert.True(group1Depth <= 2);
    }

    [Fact]
    public void MaxDepthAfterSplit_ClassicExample_MatchesStackDepthParity()
    {
        const string seq = "(()())";

        var groups = MaxDepthAfterSplit(seq);

        Assert.Equal([1, 0, 0, 0, 0, 1], groups);
    }

    private static int[] MaxDepthAfterSplit(string seq)
    {
        var groups = new int[seq.Length];
        var openers = new RepoCharStack();

        for (var i = 0; i < seq.Length; i++)
        {
            if (seq[i] == '(')
            {
                openers.Push(seq[i]);
                groups[i] = openers.Count % 2;
            }
            else
            {
                groups[i] = openers.Count % 2;
                openers.TryPop(out _);
            }
        }

        return groups;
    }

    private static string SubsequenceOf(string seq, int[] groups, int group) =>
        new([.. seq.Where((_, i) => groups[i] == group)]);

    private static int MaxDepth(string parens)
    {
        var depth = 0;
        var max = 0;

        foreach (var ch in parens)
        {
            depth += ch == '(' ? 1 : -1;
            max = Math.Max(max, depth);
        }

        return max;
    }
}
