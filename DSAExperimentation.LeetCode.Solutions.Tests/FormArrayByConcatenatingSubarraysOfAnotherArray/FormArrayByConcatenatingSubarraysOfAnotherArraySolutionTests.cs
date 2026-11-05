using DSAExperimentation.LeetCode.Solutions.FormArrayByConcatenatingSubarraysOfAnotherArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FormArrayByConcatenatingSubarraysOfAnotherArray;

// Harness only. Both strategies are
// FormArrayByConcatenatingSubarraysOfAnotherArraySolution's -
// CanChooseByNaiveSubarrayScan (previously untested scaffolding inlined in the
// benchmark as its baseline arm, where it only answered "does one pattern occur"
// rather than LeetCode's disjoint-and-in-order question) now gets the same
// cases as CanChooseByCharCompressedKmpSearch (previously this file's own
// private helper), so a failure names the strategy that broke.
public sealed partial class FormArrayByConcatenatingSubarraysOfAnotherArraySolutionTests
{
    public static TheoryData<ConcatenationCase> Cases =>
        new()
        {
            { new ConcatenationCase([[10, -2], [1, 2, 3, 4]], [1, 2, 1, 1, 2, 2, -2, 10], CanForm: false) },
            { new ConcatenationCase([[1, 2]], [1, 2], CanForm: true) },
            { new ConcatenationCase([[1, 2, 3]], [1, 2], CanForm: false) },
            { new ConcatenationCase([[1, 2], [1, 2]], [1, 2, 1, 2], CanForm: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanChooseByNaiveSubarrayScan_Cases_ReportsWhetherGroupsFitDisjointlyInOrder(
        ConcatenationCase testCase)
    {
        var actual = FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByNaiveSubarrayScan(
            testCase.Groups, testCase.Nums);

        Assert.Equal(testCase.CanForm, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanChooseByCharCompressedKmpSearch_Cases_ReportsWhetherGroupsFitDisjointlyInOrder(
        ConcatenationCase testCase)
    {
        var actual = FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByCharCompressedKmpSearch(
            testCase.Groups, testCase.Nums);

        Assert.Equal(testCase.CanForm, actual);
    }

    // One case: the groups to place, the array they must tile, and whether
    // they do it disjointly and in order. The expected value is named at every
    // construction site, so a row reads as the case it is rather than as a bare `true`
    // whose meaning is its position. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct ConcatenationCase(int[][] Groups, int[] Nums, bool CanForm);
}
