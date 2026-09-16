using DSAExperimentation.LeetCode.MergeTripletsToFormTargetTriplet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeTripletsToFormTargetTriplet;

// Harness only: both strategies live in MergeTripletsToFormTargetTripletSolution
// and answer the same question - the exhaustive subset search that used to be a
// benchmark-only baseline nothing asserted, and the Set<int>-tracked linear scan.
// One test method per strategy over LeetCode's published examples plus the edge
// cases the original coverage carried.
public sealed class MergeTripletsToFormTargetTripletTests
{
    public static TheoryData<TripletMergeExample> Examples =>
        new()
        {
            // LC example 1: [2,5,3] merged with [1,7,5] is [2,7,5].
            { new TripletMergeExample(Triplets: [[2, 5, 3], [1, 8, 4], [1, 7, 5]], Target: [2, 7, 5], Expected: true) },

            // LC example 2: every triplet overshoots the target's y coordinate.
            { new TripletMergeExample(Triplets: [[3, 4, 5], [4, 5, 6]], Target: [3, 2, 5], Expected: false) },

            // LC example 3: three of the four triplets each contribute one coordinate.
            { new TripletMergeExample(Triplets: [[2, 5, 3], [2, 3, 4], [1, 2, 5], [5, 2, 3]], Target: [5, 5, 5], Expected: true) },

            // Compatible throughout, but no triplet ever hits a target coordinate exactly.
            { new TripletMergeExample(Triplets: [[1, 3, 4], [2, 2, 2]], Target: [3, 4, 5], Expected: false) },

            // The incompatible triplet must be skipped, not merged, for the match to stand.
            { new TripletMergeExample(Triplets: [[9, 9, 9], [3, 4, 5]], Target: [3, 4, 5], Expected: true) },

            // A single triplet that already is the target.
            { new TripletMergeExample(Triplets: [[4, 6, 8]], Target: [4, 6, 8], Expected: true) },

            // A single triplet, one coordinate short.
            { new TripletMergeExample(Triplets: [[4, 6, 7]], Target: [4, 6, 8], Expected: false) },

            // Every coordinate is matched, but each by a different triplet.
            { new TripletMergeExample(Triplets: [[5, 1, 1], [1, 5, 1], [1, 1, 5]], Target: [5, 5, 5], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFormTargetByBruteForceSubsets_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        TripletMergeExample example)
    {
        var actual = MergeTripletsToFormTargetTripletSolution.CanFormTargetByBruteForceSubsets(
            example.Triplets, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFormTargetBySetTrackedLinearScan_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        TripletMergeExample example)
    {
        var actual = MergeTripletsToFormTargetTripletSolution.CanFormTargetBySetTrackedLinearScan(
            example.Triplets, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument. The expected answer is a bool, and a bare `true` or
    // `false` sitting third in a row does not say what it is a verdict on; naming the
    // field at each row below does.
    public readonly record struct TripletMergeExample(int[][] Triplets, int[] Target, bool Expected);
}
