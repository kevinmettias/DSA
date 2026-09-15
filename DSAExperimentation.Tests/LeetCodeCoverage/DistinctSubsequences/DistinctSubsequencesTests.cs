using DSAExperimentation.LeetCode.DistinctSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistinctSubsequences;

// Harness only. The single strategy is DistinctSubsequencesSolution's - this file
// pins it to LeetCode's published examples.
public sealed class DistinctSubsequencesTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "rabbbit", "rabbit", 3 },
            { "babgbag", "bag", 5 },
            { "abc", "abc", 1 },
            { "abc", "abcd", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumDistinctByMemoizedRecursion_LeetCodeExamples_ReturnsCount(
        string source, string target, int expected) =>
        Assert.Equal(
            expected,
            DistinctSubsequencesSolution.NumDistinctByMemoizedRecursion(
                new SourceText(source),
                new TargetPattern(target)));
}
