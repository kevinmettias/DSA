using DSAExperimentation.LeetCode.TransposeMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TransposeMatrix;

// Harness only: both strategies live in TransposeMatrixSolution. The cache-blocked
// arm was previously benchmark-only and unasserted; the tall and wide cases here are
// the ones that would expose a swapped row/column bound in it.
public sealed partial class TransposeMatrixTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], [[1, 4, 7], [2, 5, 8], [3, 6, 9]] },
            { [[1, 2, 3], [4, 5, 6]], [[1, 4], [2, 5], [3, 6]] },
            { [[1, 2], [3, 4], [5, 6]], [[1, 3, 5], [2, 4, 6]] },
            { [[7]], [[7]] },
            { [[1, 2, 3, 4]], [[1], [2], [3], [4]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TransposeByIndexSwap_LeetCodeExamples_FlipsOverMainDiagonal(int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, TransposeMatrixSolution.TransposeByIndexSwap(matrix));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TransposeByCacheBlocking_LeetCodeExamples_FlipsOverMainDiagonal(int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, TransposeMatrixSolution.TransposeByCacheBlocking(matrix));
}
