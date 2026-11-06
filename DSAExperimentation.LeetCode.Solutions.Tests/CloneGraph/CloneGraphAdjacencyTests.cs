using DSAExperimentation.LeetCode.Solutions.CloneGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CloneGraph;

public sealed partial class CloneGraphAdjacencyTests
{
    public static TheoryData<int[][]> Graphs =>
        new()
        {
            { [[2, 4], [1, 3], [2, 4], [1, 3]] },
            { [[]] },
            { [] },
        };

    [Theory]
    [MemberData(nameof(Graphs))]
    public void AdjacencyOf_CloneOfTheGraph_ReadsBackTheRows(int[][] adjacencyList)
    {
        var graph = CloneGraphAdjacency.GraphOf(adjacencyList);
        var clone = CloneGraphAdjacency.GraphOf(adjacencyList);

        Assert.Equal(adjacencyList, CloneGraphAdjacency.AdjacencyOf(clone, graph));
    }

    [Fact]
    public void AdjacencyOf_TheGraphItself_IsNoGraph()
    {
        var graph = CloneGraphAdjacency.GraphOf([[2], [1]]);

        Assert.Empty(CloneGraphAdjacency.AdjacencyOf(graph, graph));
    }
}
