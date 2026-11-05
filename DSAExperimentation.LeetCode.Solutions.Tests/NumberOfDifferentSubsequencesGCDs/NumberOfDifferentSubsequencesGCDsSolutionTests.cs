using DSAExperimentation.LeetCode.Solutions.NumberOfDifferentSubsequencesGCDs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfDifferentSubsequencesGCDs;

// Harness only. Both strategies live in NumberOfDifferentSubsequencesGCDsSolution -
// this file pins them to the singleton, the
// all-multiples chain whose achievable gcds are exactly its own values, the coprime
// pair whose gcd 1 is only reachable by taking both, and a repeated value that must
// not be counted twice.
public sealed partial class NumberOfDifferentSubsequencesGCDsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [7], 1 },
            { [4, 8, 16], 3 },
            { [2, 3], 3 },
            { [5, 5], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDifferentSubsequenceGcdsByWholeArrayScan_Cases_ReturnsDistinctAchievableGcdCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            NumberOfDifferentSubsequencesGCDsSolution.CountDifferentSubsequenceGcdsByWholeArrayScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDifferentSubsequenceGcdsBySetMultiples_Cases_ReturnsDistinctAchievableGcdCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            NumberOfDifferentSubsequencesGCDsSolution.CountDifferentSubsequenceGcdsBySetMultiples(nums));
}
