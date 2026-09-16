using DSAExperimentation.LeetCode.MaximumNestingDepthOfTwoValidParenthesesStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNestingDepthOfTwoValidParenthesesStrings;

// Harness only: both strategies live in
// MaximumNestingDepthOfTwoValidParenthesesStringsSolution and answer with the same
// depth-parity assignment, so one expected array serves both. The fully-nested
// "(((())))" case is the one the original test checked only by property (that the
// two groups' depths sum to the original's); stating the array outright is strictly
// stronger and still satisfies that property.
public sealed partial class MaximumNestingDepthOfTwoValidParenthesesStringsTests
{
    public static TheoryData<string, int[]> Examples =>
        new()
        {
            { "()", new[] { 1, 1 } },
            { "(())", new[] { 1, 0, 0, 1 } },
            { "(()())", new[] { 1, 0, 0, 0, 0, 1 } },
            { "()()", new[] { 1, 1, 1, 1 } },
            { "(((())))", new[] { 1, 0, 1, 0, 0, 1, 0, 1 } },
            { "(()(()))", new[] { 1, 0, 0, 0, 1, 1, 0, 1 } },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthAfterSplitByDepthRescan_LeetCodeExamples_AssignsGroupsByDepthParity(
        string seq,
        int[] expected) =>
        Assert.Equal(
            expected,
            MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByDepthRescan(seq));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthAfterSplitByOpenerStack_LeetCodeExamples_AssignsGroupsByDepthParity(
        string seq,
        int[] expected) =>
        Assert.Equal(
            expected,
            MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByOpenerStack(seq));
}
