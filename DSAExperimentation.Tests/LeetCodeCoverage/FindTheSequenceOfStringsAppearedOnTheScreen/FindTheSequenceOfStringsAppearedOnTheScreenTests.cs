using DSAExperimentation.LeetCode.FindTheSequenceOfStringsAppearedOnTheScreen;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheSequenceOfStringsAppearedOnTheScreen;

// Harness only. Both screen-walk strategies are
// FindTheSequenceOfStringsAppearedOnTheScreenSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class FindTheSequenceOfStringsAppearedOnTheScreenTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "abc", ["a", "aa", "ab", "aba", "abb", "abc"] },
            { "he", ["a", "b", "c", "d", "e", "f", "g", "h", "ha", "hb", "hc", "hd", "he"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void StringSequenceByStringBuilder_LeetCodeExamples_ReturnsMinimumKeyPressSequence(
        string target, string[] expected) =>
        Assert.Equal(
            expected,
            FindTheSequenceOfStringsAppearedOnTheScreenSolution.StringSequenceByStringBuilder(target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void StringSequenceByGrowableBuffer_LeetCodeExamples_ReturnsMinimumKeyPressSequence(
        string target, string[] expected) =>
        Assert.Equal(
            expected,
            FindTheSequenceOfStringsAppearedOnTheScreenSolution.StringSequenceByGrowableBuffer(target));
}
