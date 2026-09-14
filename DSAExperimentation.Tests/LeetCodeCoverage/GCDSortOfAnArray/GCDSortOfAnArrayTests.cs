using DSAExperimentation.LeetCode.GCDSortOfAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GCDSortOfAnArray;

// Harness only: both strategies live in GCDSortOfAnArraySolution and are pinned here to
// LeetCode's three published examples plus the cases the two arms have to agree on - a
// lone value, an already-sorted array whose values share no factor at all (so every
// component is a singleton and no swap is ever needed), and an array that needs exactly
// one swap across a component boundary that does not exist.
public sealed class GCDSortOfAnArrayTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [7, 21, 3], true },
            { [5, 2, 6, 2], false },
            { [10, 5, 9, 3, 15], true },
            { [2], true },
            { [2, 3, 5], true },
            { [4, 2, 3], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeSortedByPairwiseGcdUnionFind_LeetCodeExamples_ReturnsWhetherSwapsCanSortTheArray(
        int[] nums, bool expected) =>
        Assert.Equal(expected, GCDSortOfAnArraySolution.CanBeSortedByPairwiseGcdUnionFind(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeSortedByPrimeFactorDisjointSet_LeetCodeExamples_ReturnsWhetherSwapsCanSortTheArray(
        int[] nums, bool expected) =>
        Assert.Equal(expected, GCDSortOfAnArraySolution.CanBeSortedByPrimeFactorDisjointSet(nums));
}
