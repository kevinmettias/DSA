using DSAExperimentation.LeetCode.Solutions.IsGraphBipartite;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IsGraphBipartite;

public sealed partial class IsGraphBipartitePromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 2, 3], [0, 2], [0, 1, 3], [0, 2]], true },
            { [[1, 3], [0, 2], [1, 3], [0, 2]], true },
            { [[1], []], false },
            { [[1], [0, 2]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAnUndirectedGraph(int[][] graph, bool expected)
        => Assert.Equal(expected, IsGraphBipartitePromise.IsKeptBy(graph));
}
