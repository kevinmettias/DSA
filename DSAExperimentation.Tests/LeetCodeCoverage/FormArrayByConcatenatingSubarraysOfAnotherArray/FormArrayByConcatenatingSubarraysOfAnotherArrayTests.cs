using DSAExperimentation.LeetCode.FormArrayByConcatenatingSubarraysOfAnotherArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FormArrayByConcatenatingSubarraysOfAnotherArray;

// Harness only. Both strategies are
// FormArrayByConcatenatingSubarraysOfAnotherArraySolution's -
// CanChooseByNaiveSubarrayScan (previously untested scaffolding inlined in the
// benchmark as its baseline arm, where it only answered "does one pattern occur"
// rather than LeetCode's disjoint-and-in-order question) now gets the same
// examples as CanChooseByCharCompressedKmpSearch (previously this file's own
// private helper), so a failure names the strategy that broke.
public sealed class FormArrayByConcatenatingSubarraysOfAnotherArrayTests
{
    public static TheoryData<int[][], int[], bool> Examples =>
        new()
        {
            { [[1, -1, -1], [3, -2, 0]], [1, -1, 0, 1, -1, -1, 3, -2, 0], true },
            { [[10, -2], [1, 2, 3, 4]], [1, 2, 1, 1, 2, 2, -2, 10], false },
            { [[10, -2], [1, 2, 3, 4]], [1, 2, 3, 4, 10, -2], false },
            { [[1, 2, 3], [3, 4]], [7, 7, 1, 2, 3, 4, 7, 7], false },
            { [[1, 2]], [1, 2], true },
            { [[1, 2, 3]], [1, 2], false },
            { [[1, 2], [1, 2]], [1, 2, 1, 2], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanChooseByNaiveSubarrayScan_LeetCodeExamples_ReportsWhetherGroupsFitDisjointlyInOrder(
        int[][] groups, int[] nums, bool expected) =>
        Assert.Equal(
            expected,
            FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByNaiveSubarrayScan(groups, nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanChooseByCharCompressedKmpSearch_LeetCodeExamples_ReportsWhetherGroupsFitDisjointlyInOrder(
        int[][] groups, int[] nums, bool expected) =>
        Assert.Equal(
            expected,
            FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByCharCompressedKmpSearch(groups, nums));
}
