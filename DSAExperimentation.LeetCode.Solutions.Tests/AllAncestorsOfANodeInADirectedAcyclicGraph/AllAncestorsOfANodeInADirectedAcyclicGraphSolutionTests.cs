using DSAExperimentation.LeetCode.Solutions.AllAncestorsOfANodeInADirectedAcyclicGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AllAncestorsOfANodeInADirectedAcyclicGraph;

// Harness only. Both strategies - the per-node forward walk and the Kahn's-order
// DP pass - are AllAncestorsOfANodeInADirectedAcyclicGraphSolution's; this file
// pins them to the edgeless and single-chain
// cases, which are the two shapes where the topological pass has nothing to
// propagate and everything to propagate respectively.
public sealed partial class AllAncestorsOfANodeInADirectedAcyclicGraphSolutionTests
{
    public static TheoryData<int, int[][], int[][]> Cases =>
        new()
        {
            { 5, [], [[], [], [], [], []] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetAncestorsByPerNodeForwardWalk_Cases_ReturnsEachNodesAncestorsAscending(
        int nodeCount, int[][] edges, int[][] expected) =>
        Assert.Equal(
            expected,
            AllAncestorsOfANodeInADirectedAcyclicGraphSolution
                .GetAncestorsByPerNodeForwardWalk(nodeCount, edges)
                .Select(ancestors => ancestors.ToArray()));

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetAncestorsByTopologicalDpPass_Cases_ReturnsEachNodesAncestorsAscending(
        int nodeCount, int[][] edges, int[][] expected) =>
        Assert.Equal(
            expected,
            AllAncestorsOfANodeInADirectedAcyclicGraphSolution
                .GetAncestorsByTopologicalDpPass(nodeCount, edges)
                .Select(ancestors => ancestors.ToArray()));

    // One node per id, each pointing at the nodes its edges lead to, in edge order: 0 at 1 and 2, 1 at 2.
    [Fact]
    public void BuildNodes_Edges_PointsEachNodeAtTheNodesItsEdgesLeadTo()
    {
        var nodes = AllAncestorsOfANodeInADirectedAcyclicGraphSolution.BuildNodes(4, [[0, 1], [0, 2], [1, 2]]);
        var ids = nodes.Select(node => node.Id);
        var successors = nodes.Select(node => node.Neighbors.Select(neighbor => neighbor.Id).ToArray());

        Assert.Equal([0, 1, 2, 3], ids);
        Assert.Equal([[1, 2], [2], [], []], successors);
    }
}
