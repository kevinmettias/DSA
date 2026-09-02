namespace DSAExperimentation.DataStructures.Graph.Grids;

// A weighted, coordinate-carrying node - distinct from a plain weighted node
// because ShortestPath.AStar needs geometry to compute an admissible heuristic
// against, which arbitrary edge weights don't provide.
internal sealed class WeightedGridNode(int row, int col)
{
    public int Row { get; } = row;

    public int Col { get; } = col;

    public List<(int Weight, WeightedGridNode Target)> Edges { get; } = [];

    public override string ToString() => $"({Row},{Col})";
}
