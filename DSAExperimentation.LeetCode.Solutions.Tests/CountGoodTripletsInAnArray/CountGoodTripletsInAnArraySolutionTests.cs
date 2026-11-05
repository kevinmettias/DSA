using DSAExperimentation.LeetCode.Solutions.CountGoodTripletsInAnArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountGoodTripletsInAnArray;

// Harness only. Both strategies are CountGoodTripletsInAnArraySolution's -
// including the pairwise scan, which the benchmark used to own privately as its
// baseline and nothing asserted. The cases pin the counting boundaries: fewer
// nodes than a triplet needs, a rank array that is fully increasing (every triple
// good) and one with a single inversion, and a case whose nums2 is not the
// identity permutation, so a strategy that forgot to re-rank through nums2 could
// not pass by accident. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountGoodTripletsInAnArraySolutionTests
{
    public static TheoryData<int[], int[], long> Cases =>
        new()
        {
            // Two values cannot form a triplet at all.
            { [1, 0], [0, 1], 0L },

            // One value, the smallest input the sweep has to survive.
            { [0], [0], 0L },

            // Identical permutations: the ranks increase, so all C(4, 3) triples
            // are good.
            { [0, 1, 2, 3], [0, 1, 2, 3], 4L },

            // Ranks [0, 2, 1, 3, 4] - the one inversion in the middle removes
            // three of the ten triples.
            { [0, 2, 1, 3, 4], [0, 1, 2, 3, 4], 7L },

            // nums2 is not the identity, so the answer depends on re-ranking
            // nums1 through it: the ranks come out [1, 0, 2, 3].
            { [3, 1, 0, 2], [1, 3, 0, 2], 2L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountGoodTripletsByPairwiseScan_Cases_ReturnsGoodTripletCount(
        int[] nums1, int[] nums2, long expected)
    {
        var actual = CountGoodTripletsInAnArraySolution.CountGoodTripletsByPairwiseScan(nums1, nums2);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountGoodTripletsByFenwickTreeSweep_Cases_ReturnsGoodTripletCount(
        int[] nums1, int[] nums2, long expected)
    {
        var actual = CountGoodTripletsInAnArraySolution.CountGoodTripletsByFenwickTreeSweep(nums1, nums2);
        Assert.Equal(expected, actual);
    }
}
