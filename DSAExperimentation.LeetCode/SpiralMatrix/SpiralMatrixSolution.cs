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
            (row, col, direction) = StepClockwise(matrix, (row, col, direction), visited, result);
        }

        return result;
    }

    // One clockwise step of the walk: where it stands and which way it faces is the
    // whole of its state, both going in and coming back out, so it travels as one
    // argument rather than three.
    private static (int Row, int Col, int Direction) StepClockwise(
        int[][] matrix, (int Row, int Col, int Direction) walk, bool[,] visited, List<int> result)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var (row, col, direction) = walk;

        visited[row, col] = true;
        result.Add(matrix[row][col]);

        var (dRow, dCol) = Directions[direction];
        var nextRow = row + dRow;
        var nextCol = col + dCol;

        if (!IsOnGrid(nextRow, nextCol, rows, cols) || visited[nextRow, nextCol])
        {
            direction = (direction + 1) % Directions.Length;
            (dRow, dCol) = Directions[direction];
            nextRow = row + dRow;
            nextCol = col + dCol;
        }

        return (nextRow, nextCol, direction);
    }

    // Whether the cell lies on the matrix at all.
    private static bool IsOnGrid(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;

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
            (top, bottom, left, right) = WalkRing(matrix, result, (top, bottom, left, right));
        }

        return result;
    }

    // Walks the ring the four pointers currently bound - top edge, then right edge,
    // then the bottom edge and left edge only while the shrunk pointers still bound
    // them - and hands back the pointers shrunk to the ring inward of it.
    private static (int Top, int Bottom, int Left, int Right) WalkRing(
        int[][] matrix, List<int> result, (int Top, int Bottom, int Left, int Right) bounds)
    {
        var (top, bottom, left, right) = bounds;

        WalkEdge(matrix, result, (top, left, 0, 1, right - left + 1));
        top++;

        WalkEdge(matrix, result, (top, right, 1, 0, bottom - top + 1));
        right--;

        if (top <= bottom)
        {
            WalkEdge(matrix, result, (bottom, right, 0, -1, right - left + 1));
            bottom--;
        }

        if (left <= right)
        {
            WalkEdge(matrix, result, (bottom, left, -1, 0, bottom - top + 1));
            left++;
        }

        return (top, bottom, left, right);
    }

    // Appends one ring edge: `Count` cells starting at (Row, Col), each one reached
    // by stepping (DeltaRow, DeltaCol). The four edges differ only in those three,
    // so a non-positive Count walks nothing, exactly as the empty loop did.
    private static void WalkEdge(
        int[][] matrix, List<int> result, (int Row, int Col, int DeltaRow, int DeltaCol, int Count) edge)
    {
        var row = edge.Row;
        var col = edge.Col;

        for (var step = 0; step < edge.Count; step++)
        {
            result.Add(matrix[row][col]);
            row += edge.DeltaRow;
            col += edge.DeltaCol;
        }
    }
}
