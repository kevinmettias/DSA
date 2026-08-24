namespace DSAExperimentation.Graph.Algorithms.Grids;

// The shared, read-only context a GridNode carries a reference to - the same
// pattern every other topology relies on (a node knows how to find its own
// neighbors), just for adjacency computed from geometry instead of stored as an
// explicit list. GetChildren stays a pure function of the node because the node
// carries this reference; GridTopology itself stays a stateless witness like every
// other topology in this library.
internal sealed class Grid(bool[,] passable)
{
    public int Rows => passable.GetLength(0);
    public int Cols => passable.GetLength(1);

    public bool IsPassable(int row, int col)
        => row >= 0 && row < Rows && col >= 0 && col < Cols && passable[row, col];
}
