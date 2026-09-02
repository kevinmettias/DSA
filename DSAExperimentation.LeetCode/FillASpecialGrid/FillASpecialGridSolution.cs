namespace DSAExperimentation.LeetCode.FillASpecialGrid;

// LeetCode 3537. Fill a Special Grid: fill a 2^n x 2^n grid with 0..2^(2n)-1 so
// that, recursively, every quadrant's values are all smaller than the next
// quadrant clockwise from top-right (top-right < bottom-right < bottom-left <
// top-left), and each quadrant is itself special. A 1x1 grid is trivially
// special.
//
// SpecialGridByRecursiveQuadrants is that definition transcribed directly: split,
// recurse into the four quadrants in the required order, offering each one the
// next block of values. SpecialGridByBitQuadrantDigits instead computes each cell
// closed-form: reading a cell's row/column bits from most significant to least
// significant is exactly walking the same quadrant choices top-down, one level
// per bit, so the value is those choices read as base-4 digits. Both are O(size^2)
// and need nothing beyond plain arrays and bit arithmetic - there is no existing
// primitive this composes, because there is no graph, search, or ordering problem
// here to reuse one for.
internal static class FillASpecialGridSolution
{
    public static int[][] SpecialGridByRecursiveQuadrants(int n)
    {
        var size = 1 << n;
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];
        }

        FillQuadrants(grid, row: 0, col: 0, size, start: 0);
        return grid;
    }

    // Top-right gets the lowest block of values, then bottom-right, bottom-left,
    // top-left - the order the problem's own ordering constraint spells out.
    private static void FillQuadrants(int[][] grid, int row, int col, int size, int start)
    {
        if (size == 1)
        {
            grid[row][col] = start;
            return;
        }

        var half = size / 2;
        var quadrantCount = half * half;

        FillQuadrants(grid, row, col + half, half, start);
        FillQuadrants(grid, row + half, col + half, half, start + quadrantCount);
        FillQuadrants(grid, row + half, col, half, start + (2 * quadrantCount));
        FillQuadrants(grid, row, col, half, start + (3 * quadrantCount));
    }

    // (rowBit, colBit) at each level names one of the four quadrants; QuadrantDigit
    // maps it to that quadrant's base-4 digit (top-right=0, bottom-right=1,
    // bottom-left=2, top-left=3) - the same order FillQuadrants offers blocks in,
    // just read directly off the bits instead of recursing.
    private static readonly int[] QuadrantDigit = [3, 0, 2, 1];

    public static int[][] SpecialGridByBitQuadrantDigits(int n)
    {
        var size = 1 << n;
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];

            for (var col = 0; col < size; col++)
            {
                grid[row][col] = ValueAt(row, col, n);
            }
        }

        return grid;
    }

    private static int ValueAt(int row, int col, int n)
    {
        var value = 0;

        for (var level = n - 1; level >= 0; level--)
        {
            var rowBit = (row >> level) & 1;
            var colBit = (col >> level) & 1;
            value = (value * 4) + QuadrantDigit[(rowBit * 2) + colBit];
        }

        return value;
    }
}
