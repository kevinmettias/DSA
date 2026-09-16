using DSAExperimentation.LeetCode.NumberOfDifferentSubsequencesGCDs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfDifferentSubsequencesGCDs;

// Harness only. Both strategies live in NumberOfDifferentSubsequencesGCDsSolution -
// this file pins them to LeetCode's published examples plus the singleton, the
// all-multiples chain whose achievable gcds are exactly its own values, the coprime
// pair whose gcd 1 is only reachable by taking both, and a repeated value that must
// not be counted twice.
public sealed partial class NumberOfDifferentSubsequencesGCDsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [6, 10, 3], 5 },
            { [5, 15, 40, 5, 6], 7 },
            { [7], 1 },
            { [1], 1 },
            { [4, 8, 16], 3 },
            { [2, 3], 3 },
            { [5, 5], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDifferentSubsequenceGcdsByWholeArrayScan_LeetCodeExamples_ReturnsDistinctAchievableGcdCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            NumberOfDifferentSubsequencesGCDsSolution.CountDifferentSubsequenceGcdsByWholeArrayScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDifferentSubsequenceGcdsBySetMultiples_LeetCodeExamples_ReturnsDistinctAchievableGcdCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            NumberOfDifferentSubsequencesGCDsSolution.CountDifferentSubsequenceGcdsBySetMultiples(nums));
}
