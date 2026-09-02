using RowStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ZigzagGridTraversalWithSkip;

// LeetCode 3417. Zigzag Grid Traversal With Skip: flatten grid into boustrophedon
// (snake) order - row 0 left-to-right, row 1 right-to-left, and so on - then keep
// every other value of that single flattened order, starting with the first.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class ZigzagGridTraversalWithSkipSolution
{
    // The textbook approach: no data structure at all, just an index formula for
    // which column a row's i-th visit lands on, and a running "keep" flag that
    // never resets between rows - the flag, not the row boundary, is what decides
    // whether a cell survives the skip.
    public static int[] TraverseByIndexFormula(int[][] grid)
    {
        var result = new List<int>();
        var keep = true;

        for (var row = 0; row < grid.Length; row++)
        {
            var leftToRight = row % 2 == 0;
            var cols = grid[row].Length;

            for (var i = 0; i < cols; i++)
            {
                var col = leftToRight ? i : cols - 1 - i;
                keep = Collect(result, keep, grid[row][col]);
            }
        }

        return [.. result];
    }

    // Composed: every even row is read straight off the array; every odd row is
    // pushed onto this repo's own Stack<int> and popped - the same "push forward,
    // pop reversed" idiom AddBinarySolution.AddByBitStack uses to undo a walk
    // direction, applied here to undo a row instead of a digit sequence.
    public static int[] TraverseByRowStack(int[][] grid)
    {
        var result = new List<int>();
        var keep = true;

        for (var row = 0; row < grid.Length; row++)
        {
            keep = row % 2 == 0
                ? CollectForward(grid[row], result, keep)
                : CollectReversed(grid[row], result, keep);
        }

        return [.. result];
    }

    private static bool CollectForward(int[] cells, List<int> result, bool keep)
    {
        foreach (var cell in cells)
        {
            keep = Collect(result, keep, cell);
        }

        return keep;
    }

    private static bool CollectReversed(int[] cells, List<int> result, bool keep)
    {
        var reversed = new RowStack();

        foreach (var cell in cells)
        {
            reversed.Push(cell);
        }

        while (reversed.TryPop(out var cell))
        {
            keep = Collect(result, keep, cell);
        }

        return keep;
    }

    private static bool Collect(List<int> result, bool keep, int value)
    {
        if (keep)
        {
            result.Add(value);
        }

        return !keep;
    }
}
