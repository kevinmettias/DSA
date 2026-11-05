using DSAExperimentation.LeetCode.Solutions.TransposeMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TransposeMatrix;

// Harness only: both strategies live in TransposeMatrixSolution. The cache-blocked
// arm was previously benchmark-only and unasserted; the tall and wide cases here are
// the ones that would expose a swapped row/column bound in it.
public sealed partial class TransposeMatrixSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            { [[7]], [[7]] },
            { [[1, 2, 3, 4]], [[1], [2], [3], [4]] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TransposeByIndexSwap_Cases_FlipsOverMainDiagonal(int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, TransposeMatrixSolution.TransposeByIndexSwap(matrix));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TransposeByCacheBlocking_Cases_FlipsOverMainDiagonal(int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, TransposeMatrixSolution.TransposeByCacheBlocking(matrix));
}
