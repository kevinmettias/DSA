using DSAExperimentation.LeetCode.Solutions.SplitArrayWithSameAverage;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SplitArrayWithSameAverage;

// Harness only. Both strategies are SplitArrayWithSameAverageSolution's - the
// all-subsets bit-mask enumeration and the memoized subset-sum-with-a-required-
// count DP - pinned here to a single-element array (no proper non-empty split
// exists at all), a two-element split, a larger array whose total is coprime
// enough with its length that no candidate subset size even yields an integer
// target, and a nine-element array that does split. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class SplitArrayWithSameAverageSolutionTests
{
    public static TheoryData<SplitCase> Cases =>
        new()
        {
            new SplitCase([5], SplitExists: false),
            new SplitCase([2, 2], SplitExists: true),
            new SplitCase([6, 8, 18, 3, 1], SplitExists: false),
            new SplitCase([1, 2, 3, 4, 5, 6, 7, 8, 9], SplitExists: true),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanSplitBySubsetMasks_Cases_ReturnsWhetherSplitExists(SplitCase testCase) =>
        Assert.Equal(testCase.SplitExists, SplitArrayWithSameAverageSolution.CanSplitBySubsetMasks(testCase.Nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanSplitByMemoizedSubsetSum_Cases_ReturnsWhetherSplitExists(SplitCase testCase) =>
        Assert.Equal(testCase.SplitExists, SplitArrayWithSameAverageSolution.CanSplitByMemoizedSubsetSum(testCase.Nums));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct SplitCase(int[] Nums, bool SplitExists);
}
