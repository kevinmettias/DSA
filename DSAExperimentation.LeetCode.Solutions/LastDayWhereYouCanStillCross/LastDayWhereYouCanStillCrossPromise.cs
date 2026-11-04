namespace DSAExperimentation.LeetCode.Solutions.LastDayWhereYouCanStillCross;

// LC 1970's promise about its input: the days flood every cell of the grid once each, one cell a
// day. LeetCode's judge never poses an input that breaks it. A cell named twice or off the grid
// makes the strategies' day of flooding say something other than the day. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class LastDayWhereYouCanStillCrossPromise
{
    public static bool IsKeptBy(int row, int col, int[][] cells)
        => cells.Length == row * col && IsEachOnTheGridOnce(row, col, cells);

    private static bool IsEachOnTheGridOnce(int row, int col, int[][] cells)
        => cells.All(cell => IsWithin(cell[0], row) && IsWithin(cell[1], col))
            && cells.Select(cell => (cell[0], cell[1])).Distinct().Count() == cells.Length;

    // A one-based coordinate inside a side of the given length.
    private static bool IsWithin(int coordinate, int length) => coordinate >= 1 && coordinate <= length;
}
