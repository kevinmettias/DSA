using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.LeetCode.FindASafeWalkThroughAGrid;

namespace DSAExperimentation.LeetCode.Tests.FindASafeWalkThroughAGrid;

// Harness only: both strategies are FindASafeWalkThroughAGridSolution's - this
// file pins them to LeetCode's published examples, including the one where every
// path except a single detour is unsafe. The cell-cost graph the Dijkstra strategy is
// handed is asserted on its own, edge by edge.
public sealed partial class FindASafeWalkThroughAGridSolutionTests
{
    public static TheoryData<SafeWalkCase> Examples =>
        new()
        {
            { new SafeWalkCase([[0, 1, 0, 0, 0], [0, 1, 0, 1, 0], [0, 0, 0, 1, 0]], 1, IsSafe: true) },
            {
                new SafeWalkCase(
                    [[0, 1, 1, 0, 0, 0], [1, 0, 1, 0, 0, 0], [0, 1, 1, 1, 0, 1], [0, 0, 1, 0, 1, 0]],
                    3,
                    IsSafe: false)
            },
            { new SafeWalkCase([[1, 1, 1], [1, 0, 1], [1, 1, 1]], 5, IsSafe: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSafeByBruteForceArrayDijkstra_LeetCodeExamples_ReturnsWhetherFinalHealthStaysPositive(
        SafeWalkCase example)
    {
        var actual = FindASafeWalkThroughAGridSolution.IsSafeByBruteForceArrayDijkstra(
            example.Grid, example.Health);

        Assert.Equal(example.IsSafe, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSafeByWeightedGridDijkstra_LeetCodeExamples_ReturnsWhetherFinalHealthStaysPositive(
        SafeWalkCase example)
    {
        var actual = FindASafeWalkThroughAGridSolution.IsSafeByWeightedGridDijkstra(
            example.Grid, example.Health);

        Assert.Equal(example.IsSafe, actual);
    }

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

    // One LeetCode example: the grid walked, the starting health, and whether a route to
    // the last row and column keeps the health positive all the way. The expected value
    // is named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct SafeWalkCase(int[][] Grid, int Health, bool IsSafe);
}
