using DSAExperimentation.LeetCode.PermutationInString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationInString;

// Harness only. Both frequency-window strategies are
// PermutationInStringSolution's - this file just pins them to LeetCode's
// published examples plus a couple of edge cases (s1 longer than s2, and a
// match that only appears once the window has slid past the start).
public sealed class PermutationInStringTests
{
    public static TheoryData<InclusionExample> Examples =>
        new()
        {
            { new InclusionExample(Permutation: "ab", Text: "eidbaooo", Expected: true) },
            { new InclusionExample(Permutation: "ab", Text: "eidboaoo", Expected: false) },
            { new InclusionExample(Permutation: "abc", Text: "ab", Expected: false) },
            { new InclusionExample(Permutation: "adc", Text: "dcda", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPermutationByPerWindowRebuild_LeetCodeExamples_ReturnsWhetherPermutationExists(
        InclusionExample example)
    {
        var actual = PermutationInStringSolution.HasPermutationByPerWindowRebuild(
            new PermutationPattern(example.Permutation), new SearchedText(example.Text));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPermutationBySlidingWindow_LeetCodeExamples_ReturnsWhetherPermutationExists(
        InclusionExample example)
    {
        var actual = PermutationInStringSolution.HasPermutationBySlidingWindow(
            new PermutationPattern(example.Permutation), new SearchedText(example.Text));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the permutation to look for, the text to look in, and
    // whether the text contains it. Both are `string` and the question is not
    // symmetric, so the row names the roles instead of leaving two adjacent
    // positions a caller could swap with the compiler none the wiser.
    public readonly record struct InclusionExample(string Permutation, string Text, bool Expected);
}
