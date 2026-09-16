using DSAExperimentation.LeetCode.PeaksInArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PeaksInArray;

// Harness only. Both strategies are PeaksInArraySolution's - this file just
// pins them to LeetCode's published examples.
public sealed class PeaksInArrayTests
{
    public static TheoryData<int[], int[][], List<int>> Examples =>
        new()
        {
            { [3, 1, 4, 2, 5], [[2, 3, 4], [1, 0, 4]], [0] },
            { [4, 1, 4, 2, 1, 5], [[2, 2, 4], [1, 0, 2], [1, 0, 4]], [0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPeaksByBruteForce_LeetCodeExamples_ReturnsPeakCountPerQuery(
        int[] nums, int[][] queries, List<int> expected)
    {
        var actual = PeaksInArraySolution.CountPeaksByBruteForce(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPeaksByFenwickTree_LeetCodeExamples_ReturnsPeakCountPerQuery(
        int[] nums, int[][] queries, List<int> expected)
    {
        var actual = PeaksInArraySolution.CountPeaksByFenwickTree(nums, queries);

        Assert.Equal(expected, actual);
    }
}
