using DSAExperimentation.LeetCode.DistinctSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistinctSubsequences;

// Harness only. The single strategy is DistinctSubsequencesSolution's - this file
// pins it to LeetCode's published examples. Source and target are both strings and
// the match is not symmetric, so each row names which is which rather than leaving
// two interchangeable positions.
public sealed class DistinctSubsequencesTests
{
    public static TheoryData<SubsequenceExample> Examples =>
        new()
        {
            { new SubsequenceExample(Source: "rabbbit", Target: "rabbit", Expected: 3) },
            { new SubsequenceExample(Source: "babgbag", Target: "bag", Expected: 5) },
            { new SubsequenceExample(Source: "abc", Target: "abc", Expected: 1) },
            { new SubsequenceExample(Source: "abc", Target: "abcd", Expected: 0) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumDistinctByMemoizedRecursion_LeetCodeExamples_ReturnsCount(
        SubsequenceExample example)
    {
        var count = DistinctSubsequencesSolution.NumDistinctByMemoizedRecursion(
            new SourceText(example.Source),
            new TargetPattern(example.Target));

        Assert.Equal(example.Expected, count);
    }

    // One LeetCode example: the text being searched and the pattern counted inside it.
    // The two are the same type and the match is not symmetric, so the row names which
    // is which rather than leaving two interchangeable positions. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary, not
    // a type another file would import.
    public readonly record struct SubsequenceExample(string Source, string Target, int Expected);
}
