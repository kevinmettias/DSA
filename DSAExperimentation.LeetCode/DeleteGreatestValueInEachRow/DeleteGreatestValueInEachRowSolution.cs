using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.DeleteGreatestValueInEachRow;

// LeetCode 2500. Delete Greatest Value in Each Row: while the grid still has a
// column left, delete the greatest value from every row and add the largest of
// the deleted values to the answer.
//
// The two strategies differ in whether they play the deletion rounds out. The
// simulation does exactly what the statement says, rescanning every row for its
// current maximum once per round; sorting each row ascending first collapses the
// whole process, because the values deleted together on the j-th-from-last round
// are then exactly column j - so the answer is the sum, over every column, of
// that column's maximum.
internal static class DeleteGreatestValueInEachRowSolution
{
    // No row's maximum has been located yet this round.
    private const int NoIndex = -1;

    // The textbook answer: play the problem's own simulation out, finding and
    // striking each row's current maximum with a plain O(columns) scan every
    // round. Pure BCL, and O(rows * columns^2) overall - the arm the sorted
    // strategy below has to justify itself against.
    public static int DeleteGreatestValueByRepeatedRowMaxScan(int[][] grid)
    {
        var columns = grid[0].Length;
        var deleted = new bool[grid.Length, columns];
        var sum = 0;

        for (var round = 0; round < columns; round++)
        {
            var roundMax = 0;

            for (var row = 0; row < grid.Length; row++)
            {
                var rowMax = DeleteRowMax(grid[row], deleted, row);

                roundMax = Math.Max(roundMax, rowMax);
            }

            sum += roundMax;
        }

        return sum;
    }

    // Marks this row's greatest surviving value deleted and returns it. The grid
    // itself is never written to, so the caller's input survives the simulation.
    private static int DeleteRowMax(int[] row, bool[,] deleted, int rowIndex)
    {
        var maxIndex = NoIndex;

        for (var column = 0; column < row.Length; column++)
        {
            var beatsBestSurvivor = maxIndex == NoIndex || row[column] > row[maxIndex];
            var survives = !deleted[rowIndex, column];

            if (survives && beatsBestSurvivor)
            {
                maxIndex = column;
            }
        }

        deleted[rowIndex, maxIndex] = true;
        return row[maxIndex];
    }

    // This repo's own answer: sort every row with MergeSort over
    // ArrayIndexedSequence - the same composition ArrayPartitionSolution uses for
    // LC 561 - and then take one column-wise maximum pass, which is the whole
    // round structure already laid out in order. O(rows * columns * log columns).
    //
    // MergeSort sorts in place, so each row is copied first and the caller's grid
    // is left untouched.
    public static int DeleteGreatestValueByMergeSortColumnMax(int[][] grid)
    {
        var rows = grid.Select(row => row.ToArray()).ToArray();

        foreach (var row in rows)
        {
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(row));
        }

        var sum = 0;

        for (var column = 0; column < rows[0].Length; column++)
        {
            var columnMax = 0;

            foreach (var row in rows)
            {
                columnMax = Math.Max(columnMax, row[column]);
            }

            sum += columnMax;
        }

        return sum;
    }
}
