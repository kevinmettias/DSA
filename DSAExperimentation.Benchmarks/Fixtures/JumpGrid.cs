namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' MinimumNumberOfVisitedCellsInAGrid JumpGrid
// fixture: the shared, read-only context a JumpGridNode carries a reference to.
internal sealed class JumpGrid(int[][] values)
{
    public int Rows => values.Length;

    public int Cols => values[0].Length;

    public int JumpDistance(int row, int col) => values[row][col];
}
