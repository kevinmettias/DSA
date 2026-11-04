using DSAExperimentation.LeetCode.Solutions.FindKthLargestXorCoordinateValue;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindKthLargestXorCoordinateValue;

public sealed partial class FindKthLargestXorCoordinateValuePromiseTests
{
    public static TheoryData<int[][], int, bool> Inputs =>
        new()
        {
            { [[5, 2], [1, 6]], 1, true },
            { [[5, 2], [1, 6]], 4, true },
            { [[5, 2], [1, 6]], 5, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenKIsAtMostTheValueCount(int[][] matrix, int rank, bool expected)
        => Assert.Equal(expected, FindKthLargestXorCoordinateValuePromise.IsKeptBy(matrix, rank));
}
