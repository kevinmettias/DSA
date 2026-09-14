using DSAExperimentation.LeetCode.DeleteGreatestValueInEachRow;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteGreatestValueInEachRow;

// Harness only. Both strategies are DeleteGreatestValueInEachRowSolution's - the
// round-by-round simulation the statement describes and the MergeSort-then-
// column-max collapse of it - pinned here to LeetCode's published examples plus
// a single row (where every value is its own round's maximum), a grid of equal
// values (where every round ties), and a square grid whose rows start sorted the
// wrong way round.
public sealed class DeleteGreatestValueInEachRowTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 2, 4], [3, 3, 1]], 8 },
            { [[10]], 10 },
            { [[4, 1, 3]], 8 },
            { [[5, 5], [5, 5]], 10 },
            { [[3, 2, 1], [6, 5, 4], [9, 8, 7]], 24 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteGreatestValueByRepeatedRowMaxScan_LeetCodeExamples_ReturnsSumOfEachRoundsGreatestDeletion(
        int[][] grid, int expected) =>
        Assert.Equal(expected, DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByRepeatedRowMaxScan(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteGreatestValueByMergeSortColumnMax_LeetCodeExamples_ReturnsSumOfEachRoundsGreatestDeletion(
        int[][] grid, int expected) =>
        Assert.Equal(expected, DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByMergeSortColumnMax(grid));

    // Both strategies read the caller's grid; neither may leave it sorted or
    // otherwise rewritten, which the in-place MergeSort arm would do without its
    // defensive row copy.
    [Fact]
    public void DeleteGreatestValueByMergeSortColumnMax_UnsortedGrid_LeavesTheCallersGridUntouched()
    {
        int[][] grid = [[1, 2, 4], [3, 3, 1]];

        DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByMergeSortColumnMax(grid);

        Assert.Equal([1, 2, 4], grid[0]);
        Assert.Equal([3, 3, 1], grid[1]);
    }
}
