using DSAExperimentation.LeetCode.MaximumXORWithAnElementFromArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumXORWithAnElementFromArray;

// Harness only. Both strategies live in MaximumXORWithAnElementFromArraySolution -
// this file just pins them to LeetCode's published examples plus two cases the
// offline sweep in particular has to survive: a query no element qualifies for at
// all, and a pair of queries whose limits arrive in decreasing order, which only
// the sweep's own re-ordering makes answerable in one pass.
public sealed partial class MaximumXORWithAnElementFromArrayTests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            { [0, 1, 2, 3, 4], [[3, 1], [1, 3], [5, 6]], [3, 3, 7] },
            { [5, 2, 4, 6, 6, 3], [[12, 4], [8, 1], [6, 3]], [15, -1, 5] },
            { [2], [[1, 0]], [-1] },
            { [0, 1, 2, 3, 4], [[0, 4], [4, 0]], [4, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximizeXorByLinearScanPerQuery_LeetCodeExamples_ReturnsBestXorUnderEachLimit(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = MaximumXORWithAnElementFromArraySolution.MaximizeXorByLinearScanPerQuery(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximizeXorByOfflineBitTrieSweep_LeetCodeExamples_ReturnsBestXorUnderEachLimit(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = MaximumXORWithAnElementFromArraySolution.MaximizeXorByOfflineBitTrieSweep(nums, queries);

        Assert.Equal(expected, actual);
    }
}
