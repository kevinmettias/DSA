using DSAExperimentation.LeetCode.Solutions.GCDSortOfAnArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GCDSortOfAnArray;

// Harness only: both strategies live in GCDSortOfAnArraySolution and are pinned here to
// the cases the two arms have to agree on - a
// lone value, an already-sorted array whose values share no factor at all (so every
// component is a singleton and no swap is ever needed), and an array that needs exactly
// one swap across a component boundary that does not exist.
public sealed partial class GCDSortOfAnArraySolutionTests
{
    public static TheoryData<GcdSortCase> Cases =>
        new()
        {
            { new GcdSortCase([10, 5, 9, 3, 15], CanBeSorted: true) },
            { new GcdSortCase([2], CanBeSorted: true) },
            { new GcdSortCase([2, 3, 5], CanBeSorted: true) },
            { new GcdSortCase([4, 2, 3], CanBeSorted: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanBeSortedByPairwiseGcdUnionFind_Cases_ReturnsWhetherSwapsCanSortTheArray(
        GcdSortCase testCase) =>
        Assert.Equal(
            testCase.CanBeSorted,
            GCDSortOfAnArraySolution.CanBeSortedByPairwiseGcdUnionFind(testCase.Nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanBeSortedByPrimeFactorDisjointSet_Cases_ReturnsWhetherSwapsCanSortTheArray(
        GcdSortCase testCase) =>
        Assert.Equal(
            testCase.CanBeSorted,
            GCDSortOfAnArraySolution.CanBeSortedByPrimeFactorDisjointSet(testCase.Nums));

    // One case: the array to sort and whether GCD swaps can order it. The
    // expected value is named at every construction site, so a row reads as the case it
    // is rather than as a bare `true` whose meaning is its position. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct GcdSortCase(int[] Nums, bool CanBeSorted);
}
