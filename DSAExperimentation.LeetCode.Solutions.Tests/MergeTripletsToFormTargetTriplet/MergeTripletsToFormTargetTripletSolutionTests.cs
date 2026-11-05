using DSAExperimentation.LeetCode.Solutions.MergeTripletsToFormTargetTriplet;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MergeTripletsToFormTargetTriplet;

// Harness only: both strategies live in MergeTripletsToFormTargetTripletSolution
// and answer the same question - the exhaustive subset search that used to be a
// benchmark-only baseline nothing asserted, and the Set<int>-tracked linear scan.
// One test method per strategy over the edge cases the original coverage carried.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MergeTripletsToFormTargetTripletSolutionTests
{
    public static TheoryData<TripletMergeCase> Cases =>
        new()
        {
            // Compatible throughout, but no triplet ever hits a target coordinate exactly.
            { new TripletMergeCase(Triplets: [[1, 3, 4], [2, 2, 2]], Target: [3, 4, 5], Expected: false) },

            // The incompatible triplet must be skipped, not merged, for the match to stand.
            { new TripletMergeCase(Triplets: [[9, 9, 9], [3, 4, 5]], Target: [3, 4, 5], Expected: true) },

            // A single triplet that already is the target.
            { new TripletMergeCase(Triplets: [[4, 6, 8]], Target: [4, 6, 8], Expected: true) },

            // A single triplet, one coordinate short.
            { new TripletMergeCase(Triplets: [[4, 6, 7]], Target: [4, 6, 8], Expected: false) },

            // Every coordinate is matched, but each by a different triplet.
            { new TripletMergeCase(Triplets: [[5, 1, 1], [1, 5, 1], [1, 1, 5]], Target: [5, 5, 5], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanFormTargetByBruteForceSubsets_Cases_ReturnsWhetherTargetIsReachable(
        TripletMergeCase testCase)
    {
        var actual = MergeTripletsToFormTargetTripletSolution.CanFormTargetByBruteForceSubsets(
            testCase.Triplets, testCase.Target);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanFormTargetBySetTrackedLinearScan_Cases_ReturnsWhetherTargetIsReachable(
        TripletMergeCase testCase)
    {
        var actual = MergeTripletsToFormTargetTripletSolution.CanFormTargetBySetTrackedLinearScan(
            testCase.Triplets, testCase.Target);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case as one argument. The expected answer is a bool, and a bare `true` or
    // `false` sitting third in a row does not say what it is a verdict on; naming the
    // field at each row below does.
    public readonly record struct TripletMergeCase(int[][] Triplets, int[] Target, bool Expected);
}
