using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.LeetCode.Solutions.FindASafeWalkThroughAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindASafeWalkThroughAGrid;

// Harness only: both strategies are FindASafeWalkThroughAGridSolution's, and
// CatalogCaseTests holds them to LeetCode's published examples (ARCHITECTURE 17.13), the
// one where every path except a single detour is unsafe among them. The cell-cost graph
// the Dijkstra strategy is handed is asserted here on its own, edge by edge.
public sealed partial class FindASafeWalkThroughAGridSolutionTests
{
    // LeetCode's third grid: a 0 ringed by 1s. An edge costs the cell it ENTERS, listed
    // up, down, left, right: the centre's four edges out each enter a 1, while (0,1)'s
    // edge down enters the centre for 0. The corner (0,0) has only its down and right
    // neighbours.
    [Fact]
    public void BuildCellCostGraph_LeetCodeThirdExample_WeighsEachEdgeByTheCellItEnters()
    {
        var nodes = FindASafeWalkThroughAGridSolution.BuildCellCostGraph([[1, 1, 1], [1, 0, 1], [1, 1, 1]], 3, 3);
        var fromCentre = EdgesOf(nodes, (1, 1));
        var fromTopMiddle = EdgesOf(nodes, (0, 1));
        var fromCorner = EdgesOf(nodes, (0, 0));

        Assert.Equal(9, nodes.Count);
        Assert.Equal([(1, 0, 1), (1, 2, 1), (1, 1, 0), (1, 1, 2)], fromCentre);
        Assert.Equal([(0, 1, 1), (1, 0, 0), (1, 0, 2)], fromTopMiddle);
        Assert.Equal([(1, 1, 0), (1, 0, 1)], fromCorner);
    }

    private static (int Weight, int Row, int Col)[] EdgesOf(
        Dictionary<(int Row, int Col), WeightedGridNode> nodes, (int Row, int Col) cell)
        => [.. nodes[cell].Edges.Select(edge => (edge.Weight, edge.Target.Row, edge.Target.Col))];
}
