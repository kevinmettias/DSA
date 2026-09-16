using DSAExperimentation.LeetCode.SearchA2DMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchA2DMatrix;

// Harness only. Both strategies are SearchA2DMatrixSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class SearchA2DMatrixTests
{
    public static TheoryData<MatrixSearchExample> Examples =>
        new()
        {
            new MatrixSearchExample(
                Matrix: [[1, 3, 5, 7], [10, 11, 16, 20], [23, 30, 34, 60]], Target: 3, TargetExists: true),
            new MatrixSearchExample(
                Matrix: [[1, 3, 5, 7], [10, 11, 16, 20], [23, 30, 34, 60]], Target: 13, TargetExists: false),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByLinearScan_LeetCodeExamples_ReturnsWhetherTargetExists(
        MatrixSearchExample example)
    {
        var actual = SearchA2DMatrixSolution.SearchMatrixByLinearScan(example.Matrix, example.Target);

        Assert.Equal(example.TargetExists, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByBinarySearch_LeetCodeExamples_ReturnsWhetherTargetExists(
        MatrixSearchExample example)
    {
        var actual = SearchA2DMatrixSolution.SearchMatrixByBinarySearch(example.Matrix, example.Target);

        Assert.Equal(example.TargetExists, actual);
    }

    // One LeetCode example: the sorted matrix, the target to find, and whether the
    // target is present. The expectation is named rather than carried by its
    // position, so the row reads as an assertion instead of as a bare `true`.
    public readonly record struct MatrixSearchExample(int[][] Matrix, int Target, bool TargetExists);
}
