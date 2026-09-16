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
    public static TheoryData<ConcatenationCase> Examples =>
        new()
        {
            { new ConcatenationCase([[1, -1, -1], [3, -2, 0]], [1, -1, 0, 1, -1, -1, 3, -2, 0], CanForm: true) },
            { new ConcatenationCase([[10, -2], [1, 2, 3, 4]], [1, 2, 1, 1, 2, 2, -2, 10], CanForm: false) },
            { new ConcatenationCase([[10, -2], [1, 2, 3, 4]], [1, 2, 3, 4, 10, -2], CanForm: false) },
            { new ConcatenationCase([[1, 2, 3], [3, 4]], [7, 7, 1, 2, 3, 4, 7, 7], CanForm: false) },
            { new ConcatenationCase([[1, 2]], [1, 2], CanForm: true) },
            { new ConcatenationCase([[1, 2, 3]], [1, 2], CanForm: false) },
            { new ConcatenationCase([[1, 2], [1, 2]], [1, 2, 1, 2], CanForm: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanChooseByNaiveSubarrayScan_LeetCodeExamples_ReportsWhetherGroupsFitDisjointlyInOrder(
        ConcatenationCase example)
    {
        var actual = FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByNaiveSubarrayScan(
            example.Groups, example.Nums);

        Assert.Equal(example.CanForm, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanChooseByCharCompressedKmpSearch_LeetCodeExamples_ReportsWhetherGroupsFitDisjointlyInOrder(
        ConcatenationCase example)
    {
        var actual = FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByCharCompressedKmpSearch(
            example.Groups, example.Nums);

        Assert.Equal(example.CanForm, actual);
    }

    // One LeetCode example: the groups to place, the array they must tile, and whether
    // they do it disjointly and in order. The expected value is named at every
    // construction site, so a row reads as the case it is rather than as a bare `true`
    // whose meaning is its position. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct ConcatenationCase(int[][] Groups, int[] Nums, bool CanForm);
}
