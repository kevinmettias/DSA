namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfVisitedCellsInAGrid.Fixtures;

// The shared, read-only context a JumpGridNode carries a reference to - the same
// pattern DataStructures.Graph.Grids' Grid uses for GridNode/GridChildren, just reporting each
// cell's own jump distance instead of a bool passability flag.
internal sealed class JumpGrid(int[][] values)
{
    public int Rows => values.Length;

    public int Cols => values[0].Length;

    public int JumpDistance(int row, int col) => values[row][col];
}
