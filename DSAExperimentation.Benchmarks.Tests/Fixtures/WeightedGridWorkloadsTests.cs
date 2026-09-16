using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for WeightedGridWorkloads (ARCHITECTURE 17.7): the documentation's whole claim is that
// the grid is open - no walls at all - so a Manhattan heuristic is exact and A* walks the straight-line
// corridor instead of Dijkstra's full diamond. That claim is asserted directly: every cell keeps its full
// orthogonal degree, the whole grid is reachable from the source, and the far corner sits at exactly the
// Manhattan distance rather than some longer way round.
public sealed partial class WeightedGridWorkloadsTests
{
    private const int Size = 16;
    private const int GridOrigin = 0;
    private const int UnitEdgeWeight = 1;

    private static readonly (int Row, int Col)[] Orthogonal = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Fact]
    public void OpenGrid_NoWalls_LeavesEveryCellAtItsFullOrthogonalDegree()
    {
        var (source, _) = WeightedGridWorkloads.OpenGrid(Size);
        var distances = DistancesFrom(source);

        Assert.Equal(Size * Size, distances.Count);
        Assert.All(distances.Keys, node => Assert.Equal(ExpectedDegree(node), node.Edges.Count));
        Assert.All(distances.Keys, node => Assert.All(node.Edges, edge => Assert.Equal(UnitEdgeWeight, edge.Weight)));
    }

    [Fact]
    public void OpenGrid_SourceAndFarCorner_AreTheTwoOppositeCorners()
    {
        var (source, farCorner) = WeightedGridWorkloads.OpenGrid(Size);

        Assert.Equal((GridOrigin, GridOrigin), (source.Row, source.Col));
        Assert.Equal((Size - 1, Size - 1), (farCorner.Row, farCorner.Col));
    }

    // An exact heuristic is one that equals the real distance, which on an open grid is the Manhattan
    // distance - the property that makes the straight-line corridor A*'s whole exploration.
    [Fact]
    public void OpenGrid_FarCorner_SitsAtTheManhattanDistanceAlongTheEdgeWalk()
    {
        var (source, farCorner) = WeightedGridWorkloads.OpenGrid(Size);

        Assert.Equal(ManhattanDistanceToFarCorner, DistancesFrom(source)[farCorner]);
    }

    // One column and one row of unit steps between opposite corners.
    private static int ManhattanDistanceToFarCorner => (Size - 1) * 2;

    private static Dictionary<WeightedGridNode, int> DistancesFrom(WeightedGridNode source)
    {
        var distances = new Dictionary<WeightedGridNode, int> { [source] = 0 };
        var pending = new Queue<WeightedGridNode>();
        pending.Enqueue(source);

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();

            foreach (var (_, target) in node.Edges.Where(edge => !distances.ContainsKey(edge.Target)))
            {
                distances[target] = distances[node] + 1;
                pending.Enqueue(target);
            }
        }

        return distances;
    }

    private static int ExpectedDegree(WeightedGridNode node) =>
        Orthogonal.Count(step => InBounds(node.Row + step.Row, node.Col + step.Col));

    private static bool InBounds(int row, int col) =>
        row >= GridOrigin && row < Size && col >= GridOrigin && col < Size;
}
