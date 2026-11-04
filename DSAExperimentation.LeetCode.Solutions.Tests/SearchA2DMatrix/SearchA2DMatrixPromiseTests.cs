using DSAExperimentation.LeetCode.Solutions.SearchA2DMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchA2DMatrix;

public sealed partial class SearchA2DMatrixPromiseTests
{
    public static TheoryData<int[][], int, bool> Inputs =>
        new()
        {
            { [[1, 3, 5, 7], [10, 11, 16, 20], [23, 30, 34, 60]], 3, true },
            { [[1, 4, 7, 11, 15], [2, 5, 8, 12, 19]], 5, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheRowsReadAsOneSortedList(int[][] matrix, int target, bool expected)
        => Assert.Equal(expected, SearchA2DMatrixPromise.IsKeptBy(matrix, target));
}
