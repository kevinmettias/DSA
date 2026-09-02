using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.CountCellsInOverlappingHorizontalAndVerticalSubstrings;

// LeetCode 3529. Count Cells in Overlapping Horizontal and Vertical Substrings:
// a horizontal substring reads left-to-right, wrapping row to row - exactly a
// substring of the grid flattened row-major, since position k there already IS
// cell (k / cols, k % cols). A vertical substring reads top-to-bottom, wrapping
// column to column - exactly a substring of the grid flattened column-major,
// position k there being cell (k % rows, k / rows). So every occurrence of
// pattern in either flattened string is a run of covered cells; a cell counts
// once it is covered by at least one horizontal occurrence AND at least one
// vertical occurrence.
internal static class CountCellsInOverlappingHorizontalAndVerticalSubstringsSolution
{
    // Baseline: the textbook O(text.Length * pattern.Length) sliding-window
    // comparison in each flattened direction, marking every matched window's
    // cells directly - "what you'd write without this repo," no string-matching
    // primitive involved.
    public static int CountCellsByBruteForce(char[][] grid, string pattern)
    {
        var horizontalCovered = CoverByBruteForce(FlattenRowMajor(grid), pattern);
        var verticalCovered = CoverByBruteForce(FlattenColumnMajor(grid), pattern);

        return CountIntersection(grid.Length, grid[0].Length, horizontalCovered, verticalCovered);
    }

    private static bool[] CoverByBruteForce(char[] text, string pattern)
    {
        var covered = new bool[text.Length];

        for (var start = 0; start + pattern.Length <= text.Length; start++)
        {
            if (!MatchesAt(text, start, pattern))
            {
                continue;
            }

            for (var offset = 0; offset < pattern.Length; offset++)
            {
                covered[start + offset] = true;
            }
        }

        return covered;
    }

    private static bool MatchesAt(char[] text, int start, string pattern)
    {
        for (var offset = 0; offset < pattern.Length; offset++)
        {
            if (text[start + offset] != pattern[offset])
            {
                return false;
            }
        }

        return true;
    }

    // Composed: Algorithms.StringMatching.ZFunction.FindAll locates every
    // occurrence (including overlapping ones) of pattern in each flattened
    // direction in O(text.Length + pattern.Length); a difference array then
    // turns those occurrence starts into a covered mask in one more linear
    // pass, instead of re-touching every cell of every match one at a time.
    public static int CountCellsByZFunction(char[][] grid, string pattern)
    {
        var horizontalCovered = CoverByZFunction(FlattenRowMajor(grid), pattern);
        var verticalCovered = CoverByZFunction(FlattenColumnMajor(grid), pattern);

        return CountIntersection(grid.Length, grid[0].Length, horizontalCovered, verticalCovered);
    }

    private static bool[] CoverByZFunction(char[] text, string pattern)
    {
        var starts = ZFunction.FindAll(text, pattern);
        var delta = new int[text.Length + 1];

        foreach (var start in starts)
        {
            delta[start]++;
            delta[start + pattern.Length]--;
        }

        var covered = new bool[text.Length];
        var running = 0;

        for (var i = 0; i < text.Length; i++)
        {
            running += delta[i];
            covered[i] = running > 0;
        }

        return covered;
    }

    // Shared by both strategies: a cell counts once its row-major index is
    // covered horizontally and its column-major index is covered vertically.
    // Only how each covered mask above gets built differs between the two arms.
    private static int CountIntersection(int rows, int cols, bool[] horizontalCovered, bool[] verticalCovered)
    {
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var horizontalIndex = (r * cols) + c;
                var verticalIndex = (c * rows) + r;

                if (horizontalCovered[horizontalIndex] && verticalCovered[verticalIndex])
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Row-major: position k is grid[k / cols][k % cols] - "wraps to the first
    // column of the next row" once flattened this way.
    private static char[] FlattenRowMajor(char[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var flat = new char[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                flat[(r * cols) + c] = grid[r][c];
            }
        }

        return flat;
    }

    // Column-major: position k is grid[k % rows][k / rows] - "wraps to the
    // first row of the next column" once flattened this way.
    private static char[] FlattenColumnMajor(char[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var flat = new char[rows * cols];

        for (var c = 0; c < cols; c++)
        {
            for (var r = 0; r < rows; r++)
            {
                flat[(c * rows) + r] = grid[r][c];
            }
        }

        return flat;
    }
}
