using DSAExperimentation.LeetCode.Solutions.NumberOfSquarefulArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfSquarefulArrays;

// Harness only. Both strategies are NumberOfSquarefulArraysSolution's; this file
// pins them to cases that exercise duplicate
// dedup (equal values must not be counted twice) and outright rejection (no
// adjacent pair sums to a square).
public sealed partial class NumberOfSquarefulArraysSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1], 1 },
            { [2, 2], 1 },
            { [1, 2], 0 },
            { [1, 1, 1], 0 },
            { [1, 8, 17, 8], 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSquarefulPermsByFullPermutationFilter_Cases_ReturnsDistinctSquarefulArrangementCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfSquarefulArraysSolution.CountSquarefulPermsByFullPermutationFilter(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSquarefulPermsByPrunedBacktracking_Cases_ReturnsDistinctSquarefulArrangementCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfSquarefulArraysSolution.CountSquarefulPermsByPrunedBacktracking(nums));
}
