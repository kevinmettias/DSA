using DSAExperimentation.LeetCode.FindEventualSafeStates;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindEventualSafeStates;

// Harness only. Both strategies are FindEventualSafeStatesSolution's - this file pins
// them to LeetCode's published examples plus the four degenerate graphs the two
// arms disagree on most easily: a lone terminal node, a self loop, a two-node cycle,
// and a pure chain where every node is safe.
public sealed class FindEventualSafeStatesTests
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
}
