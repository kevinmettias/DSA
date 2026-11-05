using DSAExperimentation.LeetCode.Solutions.DeleteGreatestValueInEachRow;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DeleteGreatestValueInEachRow;

// Harness only. Both strategies are DeleteGreatestValueInEachRowSolution's - the
// round-by-round simulation the statement describes and the MergeSort-then-
// column-max collapse of it - pinned here to a single row (where every value is
// its own round's maximum), a grid of equal values (where every round ties), and a
// square grid whose rows start sorted the wrong way round. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class DeleteGreatestValueInEachRowSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[4, 1, 3]], 8 },
            { [[5, 5], [5, 5]], 10 },
            { [[3, 2, 1], [6, 5, 4], [9, 8, 7]], 24 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DeleteGreatestValueByRepeatedRowMaxScan_Cases_ReturnsSumOfEachRoundsGreatestDeletion(
        int[][] grid, int expected) =>
        Assert.Equal(expected, DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByRepeatedRowMaxScan(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void DeleteGreatestValueByMergeSortColumnMax_Cases_ReturnsSumOfEachRoundsGreatestDeletion(
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
