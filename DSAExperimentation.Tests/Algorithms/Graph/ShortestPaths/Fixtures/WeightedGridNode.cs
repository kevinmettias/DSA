namespace DSAExperimentation.Tests.Algorithms.Graph.ShortestPaths.Fixtures;

// A weighted, coordinate-carrying node - distinct from WeightedNode because
// ShortestPath.AStar needs geometry to compute an admissible heuristic against,
// which WeightedNode's arbitrary edge weights don't provide.
internal sealed class WeightedGridNode(string name, int row, int col)
{
    public string Name { get; } = name;
    public int Row { get; } = row;
    public int Col { get; } = col;

    public List<(int Weight, WeightedGridNode Target)> Edges { get; } = [];

    public override string ToString() => Name;
}
