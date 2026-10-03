using DSAExperimentation.LeetCode.DistinctSubsequences;

namespace DSAExperimentation.LeetCode.Tests.DistinctSubsequences;

// Harness only. Both strategies are DistinctSubsequencesSolution's - this file
// pins them to LeetCode's published examples. Source and target are both strings
// and the match is not symmetric, so each row names which is which rather than
// leaving two interchangeable positions.
public sealed partial class DistinctSubsequencesSolutionTests
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
    public void CountDistinctSubsequencesByMemoizedRecursion_LeetCodeExamples_ReturnsCount(
        SubsequenceExample example)
    {
        var count = DistinctSubsequencesSolution.CountDistinctSubsequencesByMemoizedRecursion(
            new SourceText(example.Source),
            new TargetPattern(example.Target));

        Assert.Equal(example.Expected, count);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDistinctSubsequencesByIterativeTable_LeetCodeExamples_ReturnsCount(
        SubsequenceExample example)
    {
        var count = DistinctSubsequencesSolution.CountDistinctSubsequencesByIterativeTable(
            new SourceText(example.Source),
            new TargetPattern(example.Target));

        Assert.Equal(example.Expected, count);
    }

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they count the same subsequences on every example - not merely
    // that each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDistinctSubsequences_AgreeOnEveryExample(SubsequenceExample example) =>
        Assert.Equal(
            DistinctSubsequencesSolution.CountDistinctSubsequencesByMemoizedRecursion(
                new SourceText(example.Source),
                new TargetPattern(example.Target)),
            DistinctSubsequencesSolution.CountDistinctSubsequencesByIterativeTable(
                new SourceText(example.Source),
                new TargetPattern(example.Target)));

    // One LeetCode example: the text being searched and the pattern counted inside it.
    // The two are the same type and the match is not symmetric, so the row names which
    // is which rather than leaving two interchangeable positions. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary, not
    // a type another file would import.
    public readonly record struct SubsequenceExample(string Source, string Target, int Expected);
}
