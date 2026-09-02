using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteGreatestValueInEachRow;

// LeetCode 2500. Delete Greatest Value in Each Row: sorting every row with this
// repo's MergeSort over ArrayIndexedSequence (same composition ArrayPartitionTests
// already uses) turns the problem's repeated "delete the greatest value in each
// row" simulation into a single pass - once every row is sorted ascending, the
// values deleted together on round j are exactly column j, so the answer is the
// sum, over every column index, of the maximum value in that column.
public sealed partial class DeleteGreatestValueInEachRowTests
{
    [Fact]
    public void DeleteGreatestValue_ClassicExample_ReturnsEight()
    {
        int[][] grid = [[1, 2, 4], [3, 3, 1]];

        Assert.Equal(8, DeleteGreatestValue(grid));
    }

    [Fact]
    public void DeleteGreatestValue_SingleCellGrid_ReturnsThatValue()
    {
        int[][] grid = [[10]];

        Assert.Equal(10, DeleteGreatestValue(grid));
    }

    private static int DeleteGreatestValue(int[][] grid)
    {
        foreach (var row in grid)
        {
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(row));
        }

        var sum = 0;
        var columns = grid[0].Length;

        for (var column = 0; column < columns; column++)
        {
            var columnMax = 0;

            foreach (var row in grid)
            {
                columnMax = Math.Max(columnMax, row[column]);
            }

            sum += columnMax;
        }

        return sum;
    }
}
