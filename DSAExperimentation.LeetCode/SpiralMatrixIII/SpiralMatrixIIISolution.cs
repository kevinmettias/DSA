namespace DSAExperimentation.LeetCode.SpiralMatrixIII;

// LeetCode 885. Spiral Matrix III: walk outward from (rStart, cStart) with
// growing stride lengths (1,1,2,2,3,3,...) cycling right/down/left/up, recording
// only the cells that land inside the rows x cols grid, until every cell has been
// visited once.
//
// Both strategies are the same clockwise growing-stride walk over direction
// vectors; they differ only in whether they re-derive "have I been here" per step.
// GridChildren/GridTopology model unordered orthogonal adjacency for graph walks,
// not a fixed clockwise turning order with growing strides, so there is no repo
// Representation/Operations primitive to compose here - the same conclusion Spiral
// Matrix (LC 54) and Spiral Matrix II (LC 59) already reached.
internal static class SpiralMatrixIIISolution
{
    private const int TurnsPerStride = 2;
    private const int DirectionCount = 4;

    private static readonly int[] DeltaRow = [0, 1, 0, -1];
    private static readonly int[] DeltaCol = [1, 0, -1, 0];

    // The defensive answer: guard every step against re-adding a cell with a
    // membership set, the same shape Spiral Matrix II's own baseline uses.
    // Written without this repo's primitives - a BCL HashSet tracks visited cells
    // - since the only input is four ints; there is no caller-supplied container
    // to hand this strategy instead.
    public static int[][] SpiralWalkByVisitedSet(int rows, int cols, int rStart, int cStart)
    {
        var visited = new HashSet<(int Row, int Col)>();
        visited.Add((rStart, cStart));

        return Walk(new SpiralGrid(rows, cols, rStart, cStart), visited);
    }

    // The strides are strictly increasing and each pair of turns closes one ring,
    // so the walk provably never revisits a cell: membership tracking can be
    // dropped entirely and every step is pure index arithmetic plus a bounds test.
    public static int[][] SpiralWalkByGrowingStride(int rows, int cols, int rStart, int cStart) =>
        Walk(new SpiralGrid(rows, cols, rStart, cStart), visited: null);

    private static int[][] Walk(SpiralGrid grid, HashSet<(int Row, int Col)>? visited)
    {
        List<int[]> result = [[grid.RowStart, grid.ColumnStart]];

        if (result.Count == grid.Total)
        {
            return result.ToArray();
        }

        var walk = new SpiralWalk(grid, result, visited);
        var position = new SpiralPosition { Row = grid.RowStart, Col = grid.ColumnStart };
        var stride = 1;

        while (result.Count < grid.Total)
        {
            if (WalkTurns(walk, ref position, stride))
            {
                break;
            }

            stride++;
        }

        return result.ToArray();
    }

    private static bool WalkTurns(SpiralWalk walk, ref SpiralPosition position, int stride)
    {
        for (var turn = 0; turn < TurnsPerStride; turn++)
        {
            if (WalkStride(walk, ref position, stride))
            {
                return true;
            }

            position.Direction = (position.Direction + 1) % DirectionCount;
        }

        return false;
    }

    private static bool WalkStride(SpiralWalk walk, ref SpiralPosition position, int stride)
    {
        for (var step = 0; step < stride; step++)
        {
            if (TryVisitCell(walk, ref position))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryVisitCell(SpiralWalk walk, ref SpiralPosition position)
    {
        position.Row += DeltaRow[position.Direction];
        position.Col += DeltaCol[position.Direction];

        if (!walk.Grid.Contains(position.Row, position.Col))
        {
            return false;
        }

        if (walk.Visited != null && !walk.Visited.Add((position.Row, position.Col)))
        {
            return false;
        }

        walk.Result.Add([position.Row, position.Col]);

        return walk.Result.Count == walk.Grid.Total;
    }

    private readonly record struct SpiralGrid(int Rows, int Columns, int RowStart, int ColumnStart)
    {
        public int Total => Rows * Columns;

        public bool Contains(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Columns;
    }

    private readonly record struct SpiralWalk(
        SpiralGrid Grid, List<int[]> Result, HashSet<(int Row, int Col)>? Visited);

    private struct SpiralPosition
    {
        public int Row;
        public int Col;
        public int Direction;
    }
}
