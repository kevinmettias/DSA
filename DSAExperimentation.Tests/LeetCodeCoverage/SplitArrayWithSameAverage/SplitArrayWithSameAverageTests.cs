using DSAExperimentation.LeetCode.SplitArrayWithSameAverage;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitArrayWithSameAverage;

// Harness only. Both strategies are SplitArrayWithSameAverageSolution's - the
// all-subsets bit-mask enumeration and the memoized subset-sum-with-a-required-
// count DP - pinned here to LeetCode's published examples plus a single-element
// array (no proper non-empty split exists at all), a two-element split, and a
// larger array whose total is coprime enough with its length that no candidate
// subset size even yields an integer target.
public sealed partial class SplitArrayWithSameAverageTests
{
    public static TheoryData<SplitExample> Examples =>
        new()
        {
            new SplitExample([1, 2, 3, 4, 5, 6, 7, 8], SplitExists: true),
            new SplitExample([3, 1], SplitExists: false),
            new SplitExample([5], SplitExists: false),
            new SplitExample([2, 2], SplitExists: true),
            new SplitExample([1, 2, 3], SplitExists: true),
            new SplitExample([6, 8, 18, 3, 1], SplitExists: false),
            new SplitExample([1, 2, 3, 4, 5, 6, 7, 8, 9], SplitExists: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanSplitBySubsetMasks_LeetCodeExamples_ReturnsWhetherSplitExists(SplitExample example) =>
        Assert.Equal(example.SplitExists, SplitArrayWithSameAverageSolution.CanSplitBySubsetMasks(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanSplitByMemoizedSubsetSum_LeetCodeExamples_ReturnsWhetherSplitExists(SplitExample example) =>
        Assert.Equal(example.SplitExists, SplitArrayWithSameAverageSolution.CanSplitByMemoizedSubsetSum(example.Nums));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct SplitExample(int[] Nums, bool SplitExists);
}
