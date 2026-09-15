namespace DSAExperimentation.LeetCode.LengthOfLongestVShapedDiagonalSegment;

// LeetCode 3459. Length of Longest V-Shaped Diagonal Segment: a segment starts at a
// cell valued 1, then walks one of the 4 diagonal directions following the infinite
// sequence 2, 0, 2, 0, ... and may make at most one 90-degree CLOCKWISE turn to the
// next diagonal direction (in clockwise order: north-east, south-east, south-west,
// north-west, wrapping) partway through, continuing the same alternation.
//
// No repo Topology fits: DataStructures.Graph.Grids.GridTopology only models
// orthogonal 4-neighbor adjacency for BFS-shaped search, not a directed diagonal
// walk with a per-position value obligation and a one-shot turn. Both strategies
// below are plain array composition over the raw grid instead - matching how
// OpenTheLockSolution's own baseline is deliberately primitive-free.
internal static class LengthOfLongestVShapedDiagonalSegmentSolution
{
    // Clockwise order: north-east, south-east, south-west, north-west, then back to
    // north-east - verified against LC's own worked example (a segment turning
    // south-east -> south-west at (2,4) in example 1).
    private static readonly (int DRow, int DCol)[] Directions =
    [
        (-1, 1),
        (1, 1),
        (1, -1),
        (-1, -1),
    ];

    public static int LongestLengthByBruteForceWalk(int[][] grid)
    {
        var rows = grid.Length;
        var cols = rows == 0 ? 0 : grid[0].Length;
        var best = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                if (grid[row][col] != 1)
                {
                    continue;
                }

                for (var direction = 0; direction < Directions.Length; direction++)
                {
                    var armBest = WalkArm(grid, (row, col), direction);
                    best = Math.Max(best, armBest);
                }
            }
        }

        return best;
    }

    // Walks the first arm forward one cell at a time and, at every position along
    // it, re-simulates a second forward walk after a clockwise turn - the textbook
    // way to explore "at most one turn" that shares no work between candidate turn
    // points, unlike LongestLengthByDirectionalDp's shared tables.
    private static int WalkArm(int[][] grid, (int Row, int Col) start, int direction)
    {
        var (dRow, dCol) = Directions[direction];
        var best = 0;
        var length = 0;
        var (row, col) = start;
        var value = grid[row][col];

        while (InBounds(grid, row, col) && grid[row][col] == value)
        {
            length++;
            best = Math.Max(best, length);
            best = Math.Max(best, length + WalkTurn(grid, (row, col), direction));

            value = NextExpected(value);
            row += dRow;
            col += dCol;
        }

        return best;
    }

    // WalkArm only ever turns at a cell it has just matched, so the turn's own start
    // value is that cell's value - read here rather than passed back in.
    private static int WalkTurn(int[][] grid, (int Row, int Col) turnCell, int direction)
    {
        var (dRow, dCol) = TurnedStep(direction);
        var row = turnCell.Row + dRow;
        var col = turnCell.Col + dCol;
        var expected = NextExpected(grid[turnCell.Row][turnCell.Col]);
        var length = 0;

        while (InBounds(grid, row, col) && grid[row][col] == expected)
        {
            length++;
            expected = NextExpected(expected);
            row += dRow;
            col += dCol;
        }

        return length;
    }

    // The step a walk advances by after turning clockwise from `direction`: the turn
    // is simply the next direction in the clockwise table, and its own row/column
    // steps are what the turned walk follows.
    private static (int DRow, int DCol) TurnedStep(int direction)
    {
        var turnDirection = (direction + 1) % Directions.Length;

        return Directions[turnDirection];
    }

    // The efficient composition: two O(rows*cols*4) tables built with plain nested
    // loops instead of WalkArm/WalkTurn's repeated re-simulation.
    //
    // armLength[r,c,d]: length of the longest segment that starts at a 1 and ENDS at
    // (r,c) having arrived via direction d - filled from predecessor (r-dRow,
    // c-dCol), so the row scan runs toward increasing row when d's row step is -1
    // (the predecessor is below) and toward decreasing row when it's +1.
    //
    // forwardLength[r,c,d]: length of the longest alternating run that STARTS at
    // (r,c) and extends forward along direction d - filled from the successor
    // (r+dRow, c+dCol), so it needs the opposite row scan order from armLength for
    // the same direction. This is what a turn continues into: forwardLength does
    // not require its start to be anchored to a 1, only that steps beyond it keep
    // alternating, exactly the arm-after-the-turn's own obligation.
    public static int LongestLengthByDirectionalDp(int[][] grid)
    {
        var rows = grid.Length;
        var cols = rows == 0 ? 0 : grid[0].Length;
        var armLength = new int[rows, cols, Directions.Length];
        var forwardLength = new int[rows, cols, Directions.Length];

        for (var direction = 0; direction < Directions.Length; direction++)
        {
            FillArmLength(grid, direction, armLength);
            FillForwardLength(grid, direction, forwardLength);
        }

        return Combine(grid, armLength, forwardLength);
    }

    private static void FillArmLength(int[][] grid, int direction, int[,,] armLength)
    {
        var rows = grid.Length;
        var cols = rows == 0 ? 0 : grid[0].Length;
        var (dRow, dCol) = Directions[direction];

        foreach (var row in RowScan(rows, dRow == 1 ? RowOrder.Ascending : RowOrder.Descending))
        {
            for (var col = 0; col < cols; col++)
            {
                armLength[row, col, direction] = ComputeArmLength(grid, (row, col), direction, armLength);
            }
        }
    }

    private static int ComputeArmLength(int[][] grid, (int Row, int Col) cell, int direction, int[,,] armLength)
    {
        var (row, col) = cell;

        if (grid[row][col] == 1)
        {
            return 1;
        }

        var (dRow, dCol) = Directions[direction];
        var predecessorRow = row - dRow;
        var predecessorCol = col - dCol;

        if (!InBounds(grid, predecessorRow, predecessorCol))
        {
            return 0;
        }

        var predecessorLength = armLength[predecessorRow, predecessorCol, direction];

        return IsArmContinuation(grid, (row, col), (predecessorRow, predecessorCol), predecessorLength)
            ? ContinuedArmLength(predecessorLength)
            : 0;
    }

    // Whether this cell continues the alternating arm arriving at its predecessor: the
    // predecessor holds an arm at all, and this cell holds the value that arm expects
    // next.
    private static bool IsArmContinuation(
        int[][] grid, (int Row, int Col) cell, (int Row, int Col) predecessor, int predecessorLength)
        => predecessorLength > 0 && grid[cell.Row][cell.Col] == NextExpected(grid[predecessor.Row][predecessor.Col]);

    // The arm reaching this cell runs one cell longer than the arm reaching its
    // predecessor.
    private static int ContinuedArmLength(int predecessorLength) => predecessorLength + 1;

    private static void FillForwardLength(int[][] grid, int direction, int[,,] forwardLength)
    {
        var rows = grid.Length;
        var cols = rows == 0 ? 0 : grid[0].Length;
        var (dRow, dCol) = Directions[direction];

        foreach (var row in RowScan(rows, dRow == -1 ? RowOrder.Ascending : RowOrder.Descending))
        {
            for (var col = 0; col < cols; col++)
            {
                forwardLength[row, col, direction] = ComputeForwardLength(grid, (row, col), direction, forwardLength);
            }
        }
    }

    private static int ComputeForwardLength(int[][] grid, (int Row, int Col) cell, int direction, int[,,] forwardLength)
    {
        var (row, col) = cell;
        var (dRow, dCol) = Directions[direction];
        var successorRow = row + dRow;
        var successorCol = col + dCol;

        if (!InBounds(grid, successorRow, successorCol) ||
            grid[successorRow][successorCol] != NextExpected(grid[row][col]))
        {
            return 1;
        }

        return 1 + forwardLength[successorRow, successorCol, direction];
    }

    private static int Combine(int[][] grid, int[,,] armLength, int[,,] forwardLength)
    {
        var rows = grid.Length;
        var cols = rows == 0 ? 0 : grid[0].Length;
        var lengths = (Arm: armLength, Forward: forwardLength);
        var best = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                for (var direction = 0; direction < Directions.Length; direction++)
                {
                    var candidate = BestThroughCell(grid, (row, col), direction, lengths);
                    best = Math.Max(best, candidate);
                }
            }
        }

        return best;
    }

    // The best segment through the arm that ENDS at `cell` along `direction`: the arm
    // alone, or - when the cell its clockwise turn lands on continues the alternation
    // - the arm extended by that forward run. A cell no arm reaches contributes
    // nothing, which is what the zero-length arm already reports.
    private static int BestThroughCell(
        int[][] grid,
        (int Row, int Col) cell,
        int direction,
        (int[,,] Arm, int[,,] Forward) lengths)
    {
        var arm = lengths.Arm[cell.Row, cell.Col, direction];

        if (arm == 0)
        {
            return 0;
        }

        var turnDirection = (direction + 1) % Directions.Length;
        var (dRow, dCol) = Directions[turnDirection];
        var nextRow = cell.Row + dRow;
        var nextCol = cell.Col + dCol;

        if (InBounds(grid, nextRow, nextCol) &&
            grid[nextRow][nextCol] == NextExpected(grid[cell.Row][cell.Col]))
        {
            return arm + lengths.Forward[nextRow, nextCol, turnDirection];
        }

        return arm;
    }

    // The value that must follow `value` for the alternation to continue. 1 only
    // ever appears as a segment's own first cell (nothing transitions to it), which
    // both strategies below rely on rather than re-check.
    private static int NextExpected(int value) => value switch
    {
        1 => 2,
        2 => 0,
        0 => 2,
        _ => -1,
    };

    // The row bound is tested first, so grid[0] is only reached once the grid is
    // known to have at least one row.
    private static bool InBounds(int[][] grid, int row, int col) =>
        row >= 0 && row < grid.Length && col >= 0 && col < grid[0].Length;

    // Which way FillArmLength and FillForwardLength sweep the rows is a state, not a
    // flag - the two orders are named so the call site says which one it wants.
    private enum RowOrder
    {
        Ascending,
        Descending,
    }

    private static IEnumerable<int> RowScan(int rows, RowOrder order)
    {
        if (order == RowOrder.Ascending)
        {
            for (var row = 0; row < rows; row++)
            {
                yield return row;
            }
        }
        else
        {
            for (var row = rows - 1; row >= 0; row--)
            {
                yield return row;
            }
        }
    }
}
