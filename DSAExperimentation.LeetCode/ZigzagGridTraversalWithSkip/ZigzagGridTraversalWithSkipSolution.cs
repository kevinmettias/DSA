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
        var outcome = CellOutcome.Keep;

        for (var row = 0; row < grid.Length; row++)
        {
            var leftToRight = row % 2 == 0;
            var cols = grid[row].Length;

            for (var i = 0; i < cols; i++)
            {
                var col = leftToRight ? i : MirroredColumn(cols, i);
                outcome = Collect(result, outcome, grid[row][col]);
            }
        }

        return [.. result];
    }

    // A right-to-left row visits the columns in reverse: position i reads column
    // cols - 1 - i.
    private static int MirroredColumn(int cols, int i) => cols - 1 - i;

    // Composed: every even row is read straight off the array; every odd row is
    // pushed onto this repo's own Stack<int> and popped - the same "push forward,
    // pop reversed" idiom AddBinarySolution.AddByBitStack uses to undo a walk
    // direction, applied here to undo a row instead of a digit sequence.
    public static int[] TraverseByRowStack(int[][] grid)
    {
        var result = new List<int>();
        var outcome = CellOutcome.Keep;

        for (var row = 0; row < grid.Length; row++)
        {
            var leftToRight = row % 2 == 0;
            outcome = leftToRight
                ? CollectForward(grid[row], result, outcome)
                : CollectReversed(grid[row], result, outcome);
        }

        return [.. result];
    }

    private static CellOutcome CollectForward(int[] cells, List<int> result, CellOutcome outcome)
    {
        foreach (var cell in cells)
        {
            outcome = Collect(result, outcome, cell);
        }

        return outcome;
    }

    private static CellOutcome CollectReversed(int[] cells, List<int> result, CellOutcome outcome)
    {
        var reversed = new RowStack();

        foreach (var cell in cells)
        {
            reversed.Push(cell);
        }

        while (reversed.TryPop(out var cell))
        {
            outcome = Collect(result, outcome, cell);
        }

        return outcome;
    }

    private static CellOutcome Collect(List<int> result, CellOutcome outcome, int value)
    {
        if (outcome == CellOutcome.Keep)
        {
            result.Add(value);
        }

        return outcome == CellOutcome.Keep ? CellOutcome.Skip : CellOutcome.Keep;
    }

    // Whether the cell being visited survives the skip: the flattening keeps
    // every other value of the boustrophedon order, starting with the first, so
    // the state alternates once per cell and never resets at a row boundary.
    private enum CellOutcome
    {
        Keep,
        Skip,
    }
}
