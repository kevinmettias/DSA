namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumObstacleRemovalToReachCorner.Fixtures;

// Coordinate-carrying node with mutable weighted edges - this project's own mirror
// of DSAExperimentation.Benchmarks.Fixtures.WeightedGridNode (each project owns its
// copy; neither references the other's internal types). Edge weight is the target
// cell's own obstacle flag (0 empty, 1 obstacle) rather than a uniform 1, so a
// shortest-weighted-path search directly answers "fewest obstacles removed."
internal sealed class ObstacleGridNode(int row, int col)
{
    public int Row { get; } = row;
    public int Col { get; } = col;

    public List<(int Weight, ObstacleGridNode Target)> Edges { get; } = [];
}
