using DSAExperimentation.LeetCode.FindEventualSafeStates;

namespace DSAExperimentation.LeetCode.Tests.FindEventualSafeStates;

// Harness only. Both strategies are FindEventualSafeStatesSolution's - this file pins
// them to LeetCode's published examples plus the four degenerate graphs the two
// arms disagree on most easily: a lone terminal node, a self loop, a two-node cycle,
// and a pure chain where every node is safe. The reversed graph Kahn's peel is handed
// is asserted on its own, node by node.
public sealed partial class FindEventualSafeStatesSolutionTests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[1, 2], [2, 3], [5], [0], [5], [], []], [2, 4, 5, 6] },
            { [[1, 2, 3, 4], [1, 2], [3, 4], [0, 4], []], [4] },
            { [[]], [0] },
            { [[0]], [] },
            { [[1], [0]], [] },
            { [[1], [2], []], [0, 1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EventualSafeNodesByDfsThreeColoring_LeetCodeExamples_ReturnsSortedSafeNodeIds(
        int[][] graph, int[] expected) =>
        Assert.Equal(expected, FindEventualSafeStatesSolution.EventualSafeNodesByDfsThreeColoring(graph));

    [Theory]
    [MemberData(nameof(Examples))]
    public void EventualSafeNodesByReversedKahnsTopologicalSort_LeetCodeExamples_ReturnsSortedSafeNodeIds(
        int[][] graph, int[] expected) =>
        Assert.Equal(expected, FindEventualSafeStatesSolution.EventualSafeNodesByReversedKahnsTopologicalSort(graph));

    // LeetCode's first graph with every edge turned round: node i's neighbours are the
    // nodes that point AT i, gathered in ascending source order. 0 is pointed at by 3; 1
    // by 0; 2 by 0 and 1; 3 by 1; 5 by 2 and 4; and nothing points at 4 or 6.
    [Fact]
    public void BuildReversedGraph_LeetCodeFirstExample_ListsEachNodesPredecessors()
    {
        var nodes = FindEventualSafeStatesSolution.BuildReversedGraph([[1, 2], [2, 3], [5], [0], [5], [], []]);
        var ids = nodes.Select(node => node.Id);
        var predecessors = nodes.Select(node => node.Neighbors.Select(neighbor => neighbor.Id).ToArray());

        Assert.Equal([0, 1, 2, 3, 4, 5, 6], ids);
        Assert.Equal([[3], [0], [0, 1], [1], [], [2, 4], []], predecessors);
    }
}
