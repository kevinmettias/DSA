using DSAExperimentation.LeetCode.Solutions.SearchA2DMatrixII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchA2DMatrixII;

public sealed partial class SearchA2DMatrixIIPromiseTests
{
    public static TheoryData<int[][], int, bool> Inputs =>
        new()
        {
            { [[1, 4, 7], [2, 5, 8], [3, 6, 9]], 5, true },
            { [[-1, -2], [-6, -7]], -2, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenRowsAndColumnsAscend(int[][] matrix, int target, bool expected)
        => Assert.Equal(expected, SearchA2DMatrixIIPromise.IsKeptBy(matrix, target));
}
