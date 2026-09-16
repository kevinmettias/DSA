namespace DSAExperimentation.LeetCode.SpiralMatrixII;

// LeetCode 59. Spiral Matrix II: fill a size x size matrix with 1..size^2 in spiral
// order.
//
// Both strategies visit exactly size^2 cells - the gap between them is per-cell
// overhead, not algorithm class. The direction-vector walk re-derives "have I been
// here" via a visited set on every step; the boundary-shrinking fill needs no
// lookup at all, just four fixed index-arithmetic loops per layer. There is no
// repo Representation/Operations primitive to compose here - a plain int[][] is
// the same shape this repo's Spiral Matrix (LC 54) coverage already uses.
internal static class SpiralMatrixIISolution
{
    private const int DirectionCount = 4;

    // The textbook approach: walk one cell at a time along a direction vector,
    // turning clockwise whenever the next cell would leave the bounds or has
    // already been visited. Written without this repo's primitives - a BCL
    // HashSet tracks visited cells - since the only input is size; there's no
    // caller-supplied container to hand this strategy instead.
    public static int[][] GenerateMatrixByDirectionVectorWalk(int size)
    {
        var matrix = Enumerable.Range(0, size).Select(_ => new int[size]).ToArray();
        var visited = new HashSet<(int Row, int Col)>();
        int[] deltaRow = [0, 1, 0, -1];
        int[] deltaCol = [1, 0, -1, 0];
        var context = new SpiralWalkContext(matrix, visited, deltaRow, deltaCol, size);
        var position = new SpiralPosition(0, 0, 0);

        for (var value = 1; value <= size * size; value++)
        {
            position = StepSpiral(context, position, value);
        }

        return matrix;
    }

    private static SpiralPosition StepSpiral(SpiralWalkContext context, SpiralPosition position, int value)
    {
        var (matrix, visited, deltaRow, deltaCol, size) = context;
        var (row, col, direction) = position;

        matrix[row][col] = value;
        visited.Add((row, col));

        var nextRow = row + deltaRow[direction];
        var nextCol = col + deltaCol[direction];

        if (IsBlocked(visited, nextRow, nextCol, size))
        {
            direction = (direction + 1) % DirectionCount;
            nextRow = row + deltaRow[direction];
            nextCol = col + deltaCol[direction];
        }

        return new SpiralPosition(nextRow, nextCol, direction);
    }

    // The walk turns whenever the cell ahead is off the board or already filled.
    private static bool IsBlocked(HashSet<(int Row, int Col)> visited, int row, int col, int size)
        => row < 0 || row >= size || col < 0 || col >= size || visited.Contains((row, col));

    // Boundary-shrinking: fill each ring's top/right/bottom/left edge in turn and
    // shrink the frame afterward. No membership lookup is needed at all.
    public static int[][] GenerateMatrixByBoundaryShrinking(int size)
    {
        var matrix = Enumerable.Range(0, size).Select(_ => new int[size]).ToArray();
        var value = 1;
        var bounds = new SpiralBounds { Top = 0, Bottom = size - 1, Left = 0, Right = size - 1 };

        while (bounds.Top <= bounds.Bottom && bounds.Left <= bounds.Right)
        {
            FillRing(matrix, ref value, ref bounds);
        }

        return matrix;
    }

    private static void FillRing(int[][] matrix, ref int value, ref SpiralBounds bounds)
    {
        FillTopEdge(matrix, ref value, ref bounds);
        FillRightEdge(matrix, ref value, ref bounds);
        FillBottomEdge(matrix, ref value, ref bounds);
        FillLeftEdge(matrix, ref value, ref bounds);
    }

    private static void FillTopEdge(int[][] matrix, ref int value, ref SpiralBounds bounds)
    {
        for (var c = bounds.Left; c <= bounds.Right; c++)
        {
            matrix[bounds.Top][c] = value++;
        }
        bounds.Top++;
    }

    private static void FillRightEdge(int[][] matrix, ref int value, ref SpiralBounds bounds)
    {
        for (var r = bounds.Top; r <= bounds.Bottom; r++)
        {
            matrix[r][bounds.Right] = value++;
        }
        bounds.Right--;
    }

    private static void FillBottomEdge(int[][] matrix, ref int value, ref SpiralBounds bounds)
    {
        for (var c = bounds.Right; c >= bounds.Left && bounds.Top <= bounds.Bottom; c--)
        {
            matrix[bounds.Bottom][c] = value++;
        }
        bounds.Bottom--;
    }

    private static void FillLeftEdge(int[][] matrix, ref int value, ref SpiralBounds bounds)
    {
        for (var r = bounds.Bottom; r >= bounds.Top && bounds.Left <= bounds.Right; r--)
        {
            matrix[r][bounds.Left] = value++;
        }
        bounds.Left++;
    }

    private readonly record struct SpiralWalkContext(
        int[][] Matrix, HashSet<(int Row, int Col)> Visited, int[] DeltaRow, int[] DeltaCol, int N);

    private readonly record struct SpiralPosition(int Row, int Col, int Direction);

    private sealed class SpiralBounds
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
    }
}
