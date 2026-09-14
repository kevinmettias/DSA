using DSAExperimentation.LeetCode.MergeTripletsToFormTargetTriplet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeTripletsToFormTargetTriplet;

// Harness only: both strategies live in MergeTripletsToFormTargetTripletSolution
// and answer the same question - the exhaustive subset search that used to be a
// benchmark-only baseline nothing asserted, and the Set<int>-tracked linear scan.
// One test method per strategy over LeetCode's published examples plus the edge
// cases the original coverage carried.
public sealed class MergeTripletsToFormTargetTripletTests
{
    public static TheoryData<int[][], int[], bool> Examples =>
        new()
        {
            // LC example 1: [2,5,3] merged with [1,7,5] is [2,7,5].
            { [[2, 5, 3], [1, 8, 4], [1, 7, 5]], [2, 7, 5], true },

            // LC example 2: every triplet overshoots the target's y coordinate.
            { [[3, 4, 5], [4, 5, 6]], [3, 2, 5], false },

            // LC example 3: three of the four triplets each contribute one coordinate.
            { [[2, 5, 3], [2, 3, 4], [1, 2, 5], [5, 2, 3]], [5, 5, 5], true },

            // Compatible throughout, but no triplet ever hits a target coordinate exactly.
            { [[1, 3, 4], [2, 2, 2]], [3, 4, 5], false },

            // The incompatible triplet must be skipped, not merged, for the match to stand.
            { [[9, 9, 9], [3, 4, 5]], [3, 4, 5], true },

            // A single triplet that already is the target.
            { [[4, 6, 8]], [4, 6, 8], true },

            // A single triplet, one coordinate short.
            { [[4, 6, 7]], [4, 6, 8], false },

            // Every coordinate is matched, but each by a different triplet.
            { [[5, 1, 1], [1, 5, 1], [1, 1, 5]], [5, 5, 5], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFormTargetByBruteForceSubsets_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        int[][] triplets, int[] target, bool expected) =>
        Assert.Equal(expected, MergeTripletsToFormTargetTripletSolution.CanFormTargetByBruteForceSubsets(triplets, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFormTargetBySetTrackedLinearScan_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        int[][] triplets, int[] target, bool expected) =>
        Assert.Equal(expected, MergeTripletsToFormTargetTripletSolution.CanFormTargetBySetTrackedLinearScan(triplets, target));
}
