using DSAExperimentation.LeetCode.MaximumNestingDepthOfTwoValidParenthesesStrings;

namespace DSAExperimentation.LeetCode.Tests.MaximumNestingDepthOfTwoValidParenthesesStrings;

// Harness only: both strategies live in
// MaximumNestingDepthOfTwoValidParenthesesStringsSolution. LeetCode accepts any split
// whose two halves are valid parentheses strings and whose deeper half is as shallow
// as possible - its own example 1 answer is the complement of the one both strategies
// give - so a row states one valid split, and a strategy's answer is held to the same
// validity and the same deeper-half depth rather than to that split's exact labels.
public sealed partial class MaximumNestingDepthOfTwoValidParenthesesStringsSolutionTests
{
    // The answer labels each character 0 or 1: the two subsequences the split makes.
    private const int GroupCount = 2;

    public static TheoryData<string, int[]> Examples =>
        new()
        {
            // LeetCode examples 1 and 2, with the outputs LeetCode publishes.
            { "(()())", new[] { 0, 1, 1, 1, 1, 0 } },
            { "()(())()", new[] { 0, 0, 0, 1, 1, 0, 1, 1 } },

            // Splits by depth parity, each by hand: a character at odd depth joins
            // group 1, at even depth group 0, so a string d deep leaves each half at most
            // ceil(d / 2) deep - and no split does better, since the d brackets open at
            // the deepest point divide between two halves.
            { "()", new[] { 1, 1 } },
            { "(())", new[] { 1, 0, 0, 1 } },
            { "(()())", new[] { 1, 0, 0, 0, 0, 1 } },
            { "()()", new[] { 1, 1, 1, 1 } },
            { "(((())))", new[] { 1, 0, 1, 0, 0, 1, 0, 1 } },
            { "(()(()))", new[] { 1, 0, 0, 0, 1, 1, 0, 1 } },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthAfterSplitByDepthRescan_LeetCodeExamples_SplitsIntoTwoMinimallyDeepHalves(
        string seq,
        int[] validSplit) =>
        AssertMinimalSplit(
            seq,
            validSplit,
            MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByDepthRescan(seq));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthAfterSplitByOpenerStack_LeetCodeExamples_SplitsIntoTwoMinimallyDeepHalves(
        string seq,
        int[] validSplit) =>
        AssertMinimalSplit(
            seq,
            validSplit,
            MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByOpenerStack(seq));

    // The row's split must itself pass, so its deeper-half depth is a real target; the
    // answer then labels every character 0 or 1, keeps both halves valid, and matches
    // that depth.
    private static void AssertMinimalSplit(string seq, int[] validSplit, int[] answer)
    {
        var minimalDepth = DeeperHalfDepth(seq, validSplit);

        Assert.NotNull(minimalDepth);
        Assert.Equal(seq.Length, answer.Length);
        Assert.All(answer, group => Assert.InRange(group, 0, GroupCount - 1));

        var answerDepth = DeeperHalfDepth(seq, answer);

        Assert.Equal(minimalDepth, answerDepth);
    }

    // The deeper of the two halves' nesting depths, or null when either half is not a
    // valid parentheses string: each group keeps its own running depth, which may never
    // drop below zero and must end at zero.
    private static int? DeeperHalfDepth(string seq, int[] split)
    {
        var openPerGroup = new int[GroupCount];
        var deepest = 0;

        for (var index = 0; index < seq.Length; index++)
        {
            var group = split[index];
            var opensABracket = seq[index] == '(';
            openPerGroup[group] += opensABracket ? 1 : -1;

            if (openPerGroup[group] < 0)
            {
                return null;
            }

            deepest = Math.Max(deepest, openPerGroup[group]);
        }

        return openPerGroup.All(open => open == 0) ? deepest : null;
    }
}
