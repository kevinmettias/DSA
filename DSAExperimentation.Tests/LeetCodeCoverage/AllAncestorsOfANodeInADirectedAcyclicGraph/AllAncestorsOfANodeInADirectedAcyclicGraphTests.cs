using DSAExperimentation.LeetCode.AllAncestorsOfANodeInADirectedAcyclicGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllAncestorsOfANodeInADirectedAcyclicGraph;

// Harness only. Both strategies - the per-node forward walk and the Kahn's-order
// DP pass - are AllAncestorsOfANodeInADirectedAcyclicGraphSolution's; this file
// pins them to LeetCode's published examples plus the edgeless and single-chain
// cases, which are the two shapes where the topological pass has nothing to
// propagate and everything to propagate respectively.
public sealed class AllAncestorsOfANodeInADirectedAcyclicGraphTests
{
    public static TheoryData<int, int[][], int[][]> Examples =>
        new()
        {
            {
                8,
                [[0, 3], [0, 4], [1, 3], [2, 4], [2, 7], [3, 5], [3, 6], [3, 7], [4, 6]],
                [[], [], [], [0, 1], [0, 2], [0, 1, 3], [0, 1, 2, 3, 4], [0, 1, 2, 3]]
            },
            {
                5,
                [[0, 1], [0, 2], [0, 3], [0, 4], [1, 2], [1, 3], [1, 4], [2, 3], [2, 4], [3, 4]],
                [[], [0], [0, 1], [0, 1, 2], [0, 1, 2, 3]]
            },
            { 5, [], [[], [], [], [], []] },
            { 4, [[0, 1], [1, 2], [2, 3]], [[], [0], [0, 1], [0, 1, 2]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetAncestorsByPerNodeForwardWalk_LeetCodeExamples_ReturnsEachNodesAncestorsAscending(
        int nodeCount, int[][] edges, int[][] expected) =>
        Assert.Equal(
            expected,
            AllAncestorsOfANodeInADirectedAcyclicGraphSolution
                .GetAncestorsByPerNodeForwardWalk(nodeCount, edges)
                .Select(ancestors => ancestors.ToArray()));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetAncestorsByTopologicalDpPass_LeetCodeExamples_ReturnsEachNodesAncestorsAscending(
        int nodeCount, int[][] edges, int[][] expected) =>
        Assert.Equal(
            expected,
            AllAncestorsOfANodeInADirectedAcyclicGraphSolution
                .GetAncestorsByTopologicalDpPass(nodeCount, edges)
                .Select(ancestors => ancestors.ToArray()));
}
