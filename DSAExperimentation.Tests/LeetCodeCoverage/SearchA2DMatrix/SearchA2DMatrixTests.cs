using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchA2DMatrix;

public sealed partial class SearchA2DMatrixTests
{
    [Theory]
    [InlineData(3, true)]
    [InlineData(13, false)]
    public void SearchMatrix_LeetCodeExample_ReturnsWhetherTargetExists(int target, bool expected)
    {
        int[][] matrix = [[1, 3, 5, 7], [10, 11, 16, 20], [23, 30, 34, 60]];
        Assert.Equal(expected, Search(matrix, target));
    }
    private static bool Search(int[][] matrix, int target) => BinarySearch.Find<int, MatrixSequence>(new MatrixSequence(matrix), target) is not null;
    private readonly struct MatrixSequence(int[][] matrix) : IRandomAccessSequence<int> { public int Length => matrix.Length * matrix[0].Length; public int Get(int index) => matrix[index / matrix[0].Length][index % matrix[0].Length]; }
}
