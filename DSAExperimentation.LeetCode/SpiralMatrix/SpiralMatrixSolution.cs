namespace DSAExperimentation.LeetCode.SpiralMatrix;

// LeetCode 54. Spiral Matrix: read every element of a rows x cols grid in
// clockwise spiral order.
//
// The two strategies differ in how they track the frontier: a visited-cell
// grid that turns clockwise whenever the next cell is out of bounds or
// already seen (O(rows*cols) extra memory for the flags), or four boundary
// pointers that shrink inward after each ring is walked (O(1) extra memory,
// no visited tracking at all). No repo primitive applies to either shape -
// GridChildren/GridTopology model unordered orthogonal adjacency for graph
// walks, not a fixed clockwise visiting order, so forcing this through
// Grid/** would not be a genuine fit.
internal static class SpiralMatrixSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];

    // The textbook simulation: walk one cell at a time, turning clockwise
    // whenever the next cell would leave the grid or revisit one already
    // seen. Deliberately written without this repo's primitives - it is the
    // arm the boundary-pointer strategy below has to justify itself against.
    public static IList<int> SpiralOrderByVisitedGridWalk(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var visited = new bool[rows, cols];
        var result = new List<int>(rows * cols);
        var row = 0;
        var col = 0;
        var direction = 0;

        for (var i = 0; i < rows * cols; i++)
        {
            (row, col, direction) = StepClockwise(matrix, row, col, direction, visited, result);
        }

        return result;
    }

    private static (int Row, int Col, int Direction) StepClockwise(
        int[][] matrix, int row, int col, int direction, bool[,] visited, List<int> result)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;

        visited[row, col] = true;
        result.Add(matrix[row][col]);

        var (dRow, dCol) = Directions[direction];
        var nextRow = row + dRow;
        var nextCol = col + dCol;

        if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols || visited[nextRow, nextCol])
        {
            direction = (direction + 1) % Directions.Length;
            (dRow, dCol) = Directions[direction];
            nextRow = row + dRow;
            nextCol = col + dCol;
        }

        return (nextRow, nextCol, direction);
    }

    // Four boundary pointers shrink inward after each ring is fully walked -
    // no visited-cell tracking needed.
    public static IList<int> SpiralOrderByBoundaryPointerShrink(int[][] matrix)
    {
        var result = new List<int>();
        var top = 0;
        var bottom = matrix.Length - 1;
        var left = 0;
        var right = matrix[0].Length - 1;

        while (top <= bottom && left <= right)
        {
            for (var c = left; c <= right; c++)
            {
                result.Add(matrix[top][c]);
            }

            top++;

            for (var r = top; r <= bottom; r++)
            {
                result.Add(matrix[r][right]);
            }

            right--;

            if (top <= bottom)
            {
                for (var c = right; c >= left; c--)
                {
                    result.Add(matrix[bottom][c]);
                }

                bottom--;
            }

            if (left <= right)
            {
                for (var r = bottom; r >= top; r--)
                {
                    result.Add(matrix[r][left]);
                }

                left++;
            }
        }

        return result;
    }
}
