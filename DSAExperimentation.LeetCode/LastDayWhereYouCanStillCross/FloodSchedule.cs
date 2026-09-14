namespace DSAExperimentation.LeetCode.LastDayWhereYouCanStillCross;

// The day each cell of the grid turns to water, one distinct day per cell, which is
// LeetCode 1970's `cells` list turned inside out: cells[i] floods on day i + 1, and a
// cell is still land on day d exactly when its flood day is greater than d. Day 0 is
// the whole grid dry.
//
// This is not a Grid: DataStructures.Graph.Grids.Grid answers "is this cell passable"
// once and for all, while the whole problem here is that passability is a function of
// the day being probed. It fixes one problem's semantics rather than a shape, and
// answers nothing else, so it lives beside the solution instead of in Domain.
internal sealed class FloodSchedule(int[,] floodDay)
{
    public int Rows => floodDay.GetLength(0);

    public int Cols => floodDay.GetLength(1);

    // The day the last cell floods, and therefore the first day the grid is certainly
    // uncrossable - every cell has flooded by then.
    public int LastDay => Rows * Cols;

    // LeetCode states cells in 1-based (row, column) pairs, in flooding order.
    public static FloodSchedule Build(int rows, int cols, int[][] cells)
    {
        var floodDay = new int[rows, cols];

        for (var i = 0; i < cells.Length; i++)
        {
            floodDay[cells[i][0] - 1, cells[i][1] - 1] = i + 1;
        }

        return new FloodSchedule(floodDay);
    }

    public bool IsLand(int row, int col, int day) => floodDay[row, col] > day;

    public bool Contains(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;
}
