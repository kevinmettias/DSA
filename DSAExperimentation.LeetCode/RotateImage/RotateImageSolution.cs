using StackOfInt = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.RotateImage;

// LeetCode 48. Rotate Image: rotate an n x n matrix 90 degrees clockwise, in
// place. Both strategies transpose the matrix first, then differ only in how
// they reverse each row - the BCL's Array.Reverse, or this repo's own LIFO
// Stack<int>, the same digit-reversal primitive ReverseInteger composes,
// applied here to a matrix row instead of a decimal digit run.
internal static class RotateImageSolution
{
    // The textbook baseline: transpose, then let the BCL reverse each row.
    public static void RotateByArrayReverse(int[][] matrix)
    {
        Transpose(matrix);

        foreach (var row in matrix)
        {
            Array.Reverse(row);
        }
    }

    // Transpose, then reverse each row by pushing it onto this repo's own
    // Stack<int> and popping - LIFO order undoes the row without a second pass.
    public static void RotateByStackReverse(int[][] matrix)
    {
        Transpose(matrix);

        foreach (var row in matrix)
        {
            ReverseWithStack(row);
        }
    }

    private static void ReverseWithStack(int[] row)
    {
        var pushed = new StackOfInt();

        foreach (var value in row)
        {
            pushed.Push(value);
        }

        for (var i = 0; i < row.Length; i++)
        {
            pushed.TryPop(out row[i]);
        }
    }

    private static void Transpose(int[][] matrix)
    {
        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = r + 1; c < matrix.Length; c++)
            {
                (matrix[r][c], matrix[c][r]) = (matrix[c][r], matrix[r][c]);
            }
        }
    }
}
