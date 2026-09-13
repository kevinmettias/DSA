using DSAExperimentation.LeetCode.SplitArrayWithSameAverage;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitArrayWithSameAverage;

// Harness only. Both strategies are SplitArrayWithSameAverageSolution's - the
// all-subsets bit-mask enumeration and the memoized subset-sum-with-a-required-
// count DP - pinned here to LeetCode's published examples plus a single-element
// array (no proper non-empty split exists at all), a two-element split, and a
// larger array whose total is coprime enough with its length that no candidate
// subset size even yields an integer target.
public sealed class SplitArrayWithSameAverageTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, 6, 7, 8], true },
            { [3, 1], false },
            { [5], false },
            { [2, 2], true },
            { [1, 2, 3], true },
            { [6, 8, 18, 3, 1], false },
            { [1, 2, 3, 4, 5, 6, 7, 8, 9], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanSplitBySubsetMasks_LeetCodeExamples_ReturnsWhetherSplitExists(int[] nums, bool expected) =>
        Assert.Equal(expected, SplitArrayWithSameAverageSolution.CanSplitBySubsetMasks(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanSplitByMemoizedSubsetSum_LeetCodeExamples_ReturnsWhetherSplitExists(int[] nums, bool expected) =>
        Assert.Equal(expected, SplitArrayWithSameAverageSolution.CanSplitByMemoizedSubsetSum(nums));
}
