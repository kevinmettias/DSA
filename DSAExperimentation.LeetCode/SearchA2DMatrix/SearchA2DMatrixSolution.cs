using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchA2DMatrix;

// LeetCode 74. Search a 2D Matrix: each row is sorted, and the first integer
// of each row is greater than the last integer of the previous row - so the
// whole matrix, read row-major, is one sorted run.
//
// The two strategies differ only in whether they exploit that: a per-row
// linear scan, or wrapping the matrix in this repo's own MatrixSequence and
// handing it to BinarySearch.Find.
internal static class SearchA2DMatrixSolution
{
    // The textbook baseline: scan every row for target. Deliberately written
    // without this repo's search primitives - it is the arm the composed
    // solution below has to justify itself against.
    public static bool HasTargetByLinearScan(int[][] matrix, int target) =>
        matrix.Any(row => Array.IndexOf(row, target) >= 0);

    // MatrixSequence flattens the matrix into one sorted run; BinarySearch.Find
    // is then LeetCode's answer verbatim.
    public static bool HasTargetByBinarySearch(int[][] matrix, int target) =>
        BinarySearch.Find<int, MatrixSequence<int>>(new MatrixSequence<int>(matrix), target) is not null;
}
