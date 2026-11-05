using DSAExperimentation.LeetCode.Solutions.SearchA2DMatrixII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchA2DMatrixII;

// Harness only. All three strategies are SearchA2DMatrixIISolution's - this file
// pins them to the empty-matrix edge case that forces the corner walk to guard its
// first index instead of assuming a non-empty row. LeetCode's published examples run
// in CatalogCaseTests.
public sealed partial class SearchA2DMatrixIISolutionTests
{
    public static TheoryData<MatrixQuery> Cases =>
        new()
        {
            { new MatrixQuery(Matrix: [], Target: 1, Found: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasTargetByFullScan_Cases_ReturnsWhetherTargetExists(MatrixQuery query)
    {
        var actual = SearchA2DMatrixIISolution.HasTargetByFullScan(query.Matrix, query.Target);

        Assert.Equal(query.Found, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasTargetByPerRowBinarySearch_Cases_ReturnsWhetherTargetExists(MatrixQuery query)
    {
        var actual = SearchA2DMatrixIISolution.HasTargetByPerRowBinarySearch(query.Matrix, query.Target);

        Assert.Equal(query.Found, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasTargetByStaircaseSearch_Cases_ReturnsWhetherTargetExists(MatrixQuery query)
    {
        var actual = SearchA2DMatrixIISolution.HasTargetByStaircaseSearch(query.Matrix, query.Target);

        Assert.Equal(query.Found, actual);
    }

    // One case: the matrix to search, the value to look for, and whether it
    // is there. Whether the target was found is named at the row that states it, so a
    // reader of `Cases` never has to remember which position `true` sits in.
    public readonly record struct MatrixQuery(int[][] Matrix, int Target, bool Found);
}
