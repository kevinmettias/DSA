using DSAExperimentation.LeetCode.Solutions.CountIncreasingQuadruplets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountIncreasingQuadruplets;

// Harness only. Both strategies are CountIncreasingQuadrupletsSolution's - this
// file just pins them to the strictly
// decreasing permutation the original test carried and three further
// permutations that actually exercise the middle-inversion pivot (the published
// examples only reach a count of 2). Every case is a permutation of 1..n, which
// LC 2552's constraints guarantee and the Fenwick arm's value-as-index mapping
// relies on. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountIncreasingQuadrupletsSolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [5, 4, 3, 2, 1], 0L },
            { [2, 4, 1, 3, 5], 1L },
            { [3, 1, 4, 2, 5, 6], 2L },
            { [1, 4, 3, 2, 5, 6], 6L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountQuadrupletsByBruteForce_Cases_ReturnsExpectedCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountIncreasingQuadrupletsSolution.CountQuadrupletsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountQuadrupletsByFenwickTreeSweep_Cases_ReturnsExpectedCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountIncreasingQuadrupletsSolution.CountQuadrupletsByFenwickTreeSweep(nums));
}
