using DSAExperimentation.LeetCode.CountIncreasingQuadruplets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountIncreasingQuadruplets;

// Harness only. Both strategies are CountIncreasingQuadrupletsSolution's - this
// file just pins them to LeetCode's published examples, plus the strictly
// decreasing permutation the original test carried and three further
// permutations that actually exercise the middle-inversion pivot (the published
// examples only reach a count of 2). Every case is a permutation of 1..n, which
// LC 2552's constraints guarantee and the Fenwick arm's value-as-index mapping
// relies on.
public sealed class CountIncreasingQuadrupletsTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 3, 2, 4, 5], 2L },
            { [1, 2, 3, 4], 0L },
            { [5, 4, 3, 2, 1], 0L },
            { [2, 4, 1, 3, 5], 1L },
            { [3, 1, 4, 2, 5, 6], 2L },
            { [1, 4, 3, 2, 5, 6], 6L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountQuadrupletsByBruteForce_LeetCodeExamples_ReturnsExpectedCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountIncreasingQuadrupletsSolution.CountQuadrupletsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountQuadrupletsByFenwickTreeSweep_LeetCodeExamples_ReturnsExpectedCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountIncreasingQuadrupletsSolution.CountQuadrupletsByFenwickTreeSweep(nums));
}
