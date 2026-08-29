namespace DSAExperimentation.Benchmarks.Fixtures;

// Coordinate-carrying node, distinct from WeightedGraphNode: ShortestPath.AStar
// needs geometry to compute an admissible/consistent heuristic against.
internal sealed class WeightedGridNode(int row, int col)
{
    public int Row { get; } = row;
    public int Col { get; } = col;

    public List<(int Weight, WeightedGridNode Target)> Edges { get; } = [];
}
