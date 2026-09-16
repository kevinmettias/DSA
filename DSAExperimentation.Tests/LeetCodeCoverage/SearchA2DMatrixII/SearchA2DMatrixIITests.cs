using DSAExperimentation.LeetCode.SearchA2DMatrixII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchA2DMatrixII;

// Harness only. All three strategies are SearchA2DMatrixIISolution's - this file
// pins them to LeetCode's published examples, including the empty-matrix edge
// case that forces the corner walk to guard its first index instead of assuming a
// non-empty row.
public sealed class SearchA2DMatrixIITests
{
    private static readonly int[][] Matrix =
    [
        [1, 4, 7, 11, 15],
        [2, 5, 8, 12, 19],
        [3, 6, 9, 16, 22],
        [10, 13, 14, 17, 24],
        [18, 21, 23, 26, 30],
    ];

    public static TheoryData<MatrixQuery> Examples =>
        new()
        {
            { new MatrixQuery(Matrix: Matrix, Target: 5, Found: true) },
            { new MatrixQuery(Matrix: Matrix, Target: 20, Found: false) },
            { new MatrixQuery(Matrix: [], Target: 1, Found: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByFullScan_LeetCodeExamples_ReturnsWhetherTargetExists(MatrixQuery query)
    {
        var actual = SearchA2DMatrixIISolution.SearchMatrixByFullScan(query.Matrix, query.Target);

        Assert.Equal(query.Found, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByPerRowBinarySearch_LeetCodeExamples_ReturnsWhetherTargetExists(MatrixQuery query)
    {
        var actual = SearchA2DMatrixIISolution.SearchMatrixByPerRowBinarySearch(query.Matrix, query.Target);

        Assert.Equal(query.Found, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByStaircaseSearch_LeetCodeExamples_ReturnsWhetherTargetExists(MatrixQuery query)
    {
        var actual = SearchA2DMatrixIISolution.SearchMatrixByStaircaseSearch(query.Matrix, query.Target);

        Assert.Equal(query.Found, actual);
    }

    // One LeetCode example: the matrix to search, the value to look for, and whether it
    // is there. Whether the target was found is named at the row that states it, so a
    // reader of `Examples` never has to remember which position `true` sits in.
    public readonly record struct MatrixQuery(int[][] Matrix, int Target, bool Found);
}
