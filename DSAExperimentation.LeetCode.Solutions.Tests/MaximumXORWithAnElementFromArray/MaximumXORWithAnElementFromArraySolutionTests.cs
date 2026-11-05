using DSAExperimentation.LeetCode.Solutions.MaximumXORWithAnElementFromArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumXORWithAnElementFromArray;

// Harness only. Both strategies live in MaximumXORWithAnElementFromArraySolution -
// this file just pins them to two cases the
// offline sweep in particular has to survive: a query no element qualifies for at
// all, and a pair of queries whose limits arrive in decreasing order, which only
// the sweep's own re-ordering makes answerable in one pass.
public sealed partial class MaximumXORWithAnElementFromArraySolutionTests
{
    public static TheoryData<int[], int[][], int[]> Cases =>
        new()
        {
            { [2], [[1, 0]], [-1] },
            { [0, 1, 2, 3, 4], [[0, 4], [4, 0]], [4, 4] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximizeXorByLinearScanPerQuery_Cases_ReturnsBestXorUnderEachLimit(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = MaximumXORWithAnElementFromArraySolution.MaximizeXorByLinearScanPerQuery(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximizeXorByOfflineBitTrieSweep_Cases_ReturnsBestXorUnderEachLimit(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = MaximumXORWithAnElementFromArraySolution.MaximizeXorByOfflineBitTrieSweep(nums, queries);

        Assert.Equal(expected, actual);
    }
}
