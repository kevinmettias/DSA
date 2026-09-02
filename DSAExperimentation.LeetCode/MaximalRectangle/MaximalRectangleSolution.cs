using HeightStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaximalRectangle;

// LeetCode 85. Maximal Rectangle: the largest all-'1's rectangle in a binary
// matrix.
//
// The two strategies differ in how they collapse the 2D search. RowPairScan
// is the textbook O(rows^2 * cols) approach: for every top/bottom row pair,
// collapse the rows in between into a per-column "all ones in this vertical
// strip" flag, then scan for the widest contiguous run of set columns.
// RowHistogramStack reduces the problem to LeetCode 84 applied once per row -
// each row's running height array (consecutive '1's stacked on top of the
// row above) turns the search into rowCount independent histogram-max-
// rectangle sweeps, using this repo's own Stack<int> exactly as
// LargestRectangleInHistogramSolution does. The reduction is kept self-
// contained here rather than calling that solution directly - solution
// classes are one per problem, not a shared library between them.
internal static class MaximalRectangleSolution
{
    // The textbook brute force: for every pair of rows, collapse the strip
    // between them into "is this column all ones from top to bottom" and
    // scan for the widest run. Deliberately written without this repo's
    // primitives - it is the arm the reduction below has to justify itself
    // against.
    public static int MaximalRectangleAreaByRowPairScan(char[][] matrix)
    {
        if (matrix.Length == 0)
        {
            return 0;
        }

        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var maxArea = 0;

        for (var top = 0; top < rows; top++)
        {
            var columnAllOnes = new bool[cols];
            Array.Fill(columnAllOnes, true);

            for (var bottom = top; bottom < rows; bottom++)
            {
                var rowPairArea = ScanRowPair(matrix, top, bottom, columnAllOnes);
                maxArea = Math.Max(maxArea, rowPairArea);
            }
        }

        return maxArea;
    }

    private static int ScanRowPair(char[][] matrix, int top, int bottom, bool[] columnAllOnes)
    {
        var cols = columnAllOnes.Length;

        for (var c = 0; c < cols; c++)
        {
            columnAllOnes[c] &= matrix[bottom][c] == '1';
        }

        var height = bottom - top + 1;
        var run = 0;
        var maxArea = 0;

        for (var c = 0; c < cols; c++)
        {
            run = columnAllOnes[c] ? run + 1 : 0;
            maxArea = Math.Max(maxArea, run * height);
        }

        return maxArea;
    }

    // Row-by-row reduction to LC 84: each row's running height array turns
    // one row into a histogram, so the largest rectangle found by the same
    // monotonic-stack sweep, maximized over every row, is the answer.
    public static int MaximalRectangleAreaByRowHistogramStack(char[][] matrix)
    {
        if (matrix.Length == 0)
        {
            return 0;
        }

        var heights = new int[matrix[0].Length];
        var maxArea = 0;

        foreach (var row in matrix)
        {
            for (var col = 0; col < row.Length; col++)
            {
                heights[col] = row[col] == '1' ? heights[col] + 1 : 0;
            }

            maxArea = Math.Max(maxArea, LargestRectangleArea(heights));
        }

        return maxArea;
    }

    private static int LargestRectangleArea(int[] heights)
    {
        var indices = new HeightStack();
        var maxArea = 0;

        for (var i = 0; i <= heights.Length; i++)
        {
            var currentHeight = i == heights.Length ? 0 : heights[i];

            while (indices.TryPeek(out var top) && heights[top] >= currentHeight)
            {
                indices.TryPop(out _);
                var height = heights[top];
                var width = indices.TryPeek(out var left) ? i - left - 1 : i;
                maxArea = Math.Max(maxArea, height * width);
            }

            indices.Push(i);
        }

        return maxArea;
    }
}
