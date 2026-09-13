using DSAExperimentation.LeetCode.NumberOfSquarefulArrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfSquarefulArrays;

// Harness only. Both strategies are NumberOfSquarefulArraysSolution's; this file
// pins them to LeetCode's published examples plus cases that exercise duplicate
// dedup (equal values must not be counted twice) and outright rejection (no
// adjacent pair sums to a square).
public sealed class NumberOfSquarefulArraysTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 17, 8], 2 },
            { [2, 2, 2], 1 },
            { [1], 1 },
            { [2, 2], 1 },
            { [1, 2], 0 },
            { [1, 1, 1], 0 },
            { [1, 8, 17, 8], 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumSquarefulPermsByFullPermutationFilter_LeetCodeExamples_ReturnsDistinctSquarefulArrangementCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfSquarefulArraysSolution.NumSquarefulPermsByFullPermutationFilter(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumSquarefulPermsByPrunedBacktracking_LeetCodeExamples_ReturnsDistinctSquarefulArrangementCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfSquarefulArraysSolution.NumSquarefulPermsByPrunedBacktracking(nums));
}
