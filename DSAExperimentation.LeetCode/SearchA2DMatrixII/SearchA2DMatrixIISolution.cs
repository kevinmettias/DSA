using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchA2DMatrixII;

// LeetCode 240. Search a 2D Matrix II: each row is independently sorted
// ascending (unlike LC 74, rows are not chained end-to-start), so the whole
// matrix is not one sorted run and per-row binary search is the strongest
// generic-primitive composition available. The staircase walk is the
// specialized O(rows+cols) corner search this problem is famous for - it
// beats per-row binary search asymptotically, the same lesson FloydWarshall
// teaches in the shortest-path cluster: a general primitive is not always
// the asymptotically optimal tool.
internal static class SearchA2DMatrixIISolution
{
    // The textbook baseline: check every cell. Deliberately written without
    // this repo's primitives - it is the arm the other two strategies
    // justify themselves against.
    public static bool SearchMatrixByFullScan(int[][] matrix, int target)
    {
        foreach (var row in matrix)
        {
            foreach (var value in row)
            {
                if (value == target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Each row is independently sorted ascending, so this repo's own
    // BinarySearch.Find over an ArraySequence<int> witness finds a target
    // within one row in O(log cols) - O(rows * log cols) overall.
    public static bool SearchMatrixByPerRowBinarySearch(int[][] matrix, int target)
    {
        foreach (var row in matrix)
        {
            var sequence = new ArraySequence<int>(row);
            if (BinarySearch.Find(sequence, target) is not null)
            {
                return true;
            }
        }

        return false;
    }

    // The specialized O(rows+cols) corner walk: start at the top-right
    // corner, step left when the current value is too big (eliminating that
    // column - everything below it in this column is even bigger), step
    // down when it's too small (eliminating that row).
    public static bool SearchMatrixByStaircaseSearch(int[][] matrix, int target)
    {
        if (matrix.Length == 0)
        {
            return false;
        }

        var row = 0;
        var col = matrix[0].Length - 1;

        while (row < matrix.Length && col >= 0)
        {
            var current = matrix[row][col];

            if (current == target)
            {
                return true;
            }

            if (current > target)
            {
                col--;
            }
            else
            {
                row++;
            }
        }

        return false;
    }
}
