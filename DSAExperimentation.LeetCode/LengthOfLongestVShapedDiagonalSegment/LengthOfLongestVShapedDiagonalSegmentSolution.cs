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
                    best = Math.Max(best, WalkArm(grid, rows, cols, row, col, direction));
                }
            }
        }

        return best;
    }

    // Walks the first arm forward one cell at a time and, at every position along
    // it, re-simulates a second forward walk after a clockwise turn - the textbook
    // way to explore "at most one turn" that shares no work between candidate turn
    // points, unlike LongestLengthByDirectionalDp's shared tables.
    private static int WalkArm(int[][] grid, int rows, int cols, int startRow, int startCol, int direction)
    {
        var (dRow, dCol) = Directions[direction];
        var best = 0;
        var length = 0;
        var row = startRow;
        var col = startCol;
        var value = grid[startRow][startCol];

        while (InBounds(row, col, rows, cols) && grid[row][col] == value)
        {
            length++;
            best = Math.Max(best, length);
            best = Math.Max(best, length + WalkTurn(grid, rows, cols, row, col, direction, value));

            value = NextExpected(value);
            row += dRow;
            col += dCol;
        }

        return best;
    }

    private static int WalkTurn(int[][] grid, int rows, int cols, int turnRow, int turnCol, int direction, int turnValue)
    {
        var turnDirection = (direction + 1) % Directions.Length;
        var (dRow, dCol) = Directions[turnDirection];
        var row = turnRow + dRow;
        var col = turnCol + dCol;
        var expected = NextExpected(turnValue);
        var length = 0;

        while (InBounds(row, col, rows, cols) && grid[row][col] == expected)
        {
            length++;
            expected = NextExpected(expected);
            row += dRow;
            col += dCol;
        }

        return length;
    }

    private static bool InBounds(int row, int col, int rows, int cols) =>
        row >= 0 && row < rows && col >= 0 && col < cols;

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
            FillArmLength(grid, rows, cols, direction, armLength);
            FillForwardLength(grid, rows, cols, direction, forwardLength);
        }

        return Combine(grid, rows, cols, armLength, forwardLength);
    }

    private static void FillArmLength(int[][] grid, int rows, int cols, int direction, int[,,] armLength)
    {
        var (dRow, dCol) = Directions[direction];

        foreach (var row in RowScan(rows, ascending: dRow == 1))
        {
            for (var col = 0; col < cols; col++)
            {
                armLength[row, col, direction] = ComputeArmLength(grid, rows, cols, row, col, direction, dRow, dCol, armLength);
            }
        }
    }

    private static int ComputeArmLength(
        int[][] grid, int rows, int cols, int row, int col, int direction, int dRow, int dCol, int[,,] armLength)
    {
        if (grid[row][col] == 1)
        {
            return 1;
        }

        var predecessorRow = row - dRow;
        var predecessorCol = col - dCol;

        if (!InBounds(predecessorRow, predecessorCol, rows, cols))
        {
            return 0;
        }

        var predecessorLength = armLength[predecessorRow, predecessorCol, direction];

        return predecessorLength > 0 && grid[row][col] == NextExpected(grid[predecessorRow][predecessorCol])
            ? predecessorLength + 1
            : 0;
    }

    private static void FillForwardLength(int[][] grid, int rows, int cols, int direction, int[,,] forwardLength)
    {
        var (dRow, dCol) = Directions[direction];

        foreach (var row in RowScan(rows, ascending: dRow == -1))
        {
            for (var col = 0; col < cols; col++)
            {
                forwardLength[row, col, direction] = ComputeForwardLength(grid, rows, cols, row, col, direction, dRow, dCol, forwardLength);
            }
        }
    }

    private static int ComputeForwardLength(
        int[][] grid, int rows, int cols, int row, int col, int direction, int dRow, int dCol, int[,,] forwardLength)
    {
        var successorRow = row + dRow;
        var successorCol = col + dCol;

        if (!InBounds(successorRow, successorCol, rows, cols) ||
            grid[successorRow][successorCol] != NextExpected(grid[row][col]))
        {
            return 1;
        }

        return 1 + forwardLength[successorRow, successorCol, direction];
    }

    private static IEnumerable<int> RowScan(int rows, bool ascending)
    {
        if (ascending)
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

    private static int Combine(int[][] grid, int rows, int cols, int[,,] armLength, int[,,] forwardLength)
    {
        var best = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                for (var direction = 0; direction < Directions.Length; direction++)
                {
                    var arm = armLength[row, col, direction];

                    if (arm == 0)
                    {
                        continue;
                    }

                    best = Math.Max(best, arm);

                    var turnDirection = (direction + 1) % Directions.Length;
                    var (dRow, dCol) = Directions[turnDirection];
                    var nextRow = row + dRow;
                    var nextCol = col + dCol;

                    if (InBounds(nextRow, nextCol, rows, cols) &&
                        grid[nextRow][nextCol] == NextExpected(grid[row][col]))
                    {
                        best = Math.Max(best, arm + forwardLength[nextRow, nextCol, turnDirection]);
                    }
                }
            }
        }

        return best;
    }
}
