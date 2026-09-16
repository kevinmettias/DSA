using DSAExperimentation.LeetCode.GCDSortOfAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GCDSortOfAnArray;

// Harness only: both strategies live in GCDSortOfAnArraySolution and are pinned here to
// LeetCode's three published examples plus the cases the two arms have to agree on - a
// lone value, an already-sorted array whose values share no factor at all (so every
// component is a singleton and no swap is ever needed), and an array that needs exactly
// one swap across a component boundary that does not exist.
public sealed partial class GCDSortOfAnArrayTests
{
    public static TheoryData<GcdSortCase> Examples =>
        new()
        {
            { new GcdSortCase([7, 21, 3], CanBeSorted: true) },
            { new GcdSortCase([5, 2, 6, 2], CanBeSorted: false) },
            { new GcdSortCase([10, 5, 9, 3, 15], CanBeSorted: true) },
            { new GcdSortCase([2], CanBeSorted: true) },
            { new GcdSortCase([2, 3, 5], CanBeSorted: true) },
            { new GcdSortCase([4, 2, 3], CanBeSorted: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeSortedByPairwiseGcdUnionFind_LeetCodeExamples_ReturnsWhetherSwapsCanSortTheArray(
        GcdSortCase example) =>
        Assert.Equal(
            example.CanBeSorted,
            GCDSortOfAnArraySolution.CanBeSortedByPairwiseGcdUnionFind(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeSortedByPrimeFactorDisjointSet_LeetCodeExamples_ReturnsWhetherSwapsCanSortTheArray(
        GcdSortCase example) =>
        Assert.Equal(
            example.CanBeSorted,
            GCDSortOfAnArraySolution.CanBeSortedByPrimeFactorDisjointSet(example.Nums));

    // One LeetCode example: the array to sort and whether GCD swaps can order it. The
    // expected value is named at every construction site, so a row reads as the case it
    // is rather than as a bare `true` whose meaning is its position. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct GcdSortCase(int[] Nums, bool CanBeSorted);
}
