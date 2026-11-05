using DSAExperimentation.LeetCode.Solutions.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

public sealed partial class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreePromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 5, [[0, 1, 1], [1, 2, 1], [2, 3, 2], [0, 3, 2], [0, 4, 3], [3, 4, 3], [1, 4, 6]], true },
            { 4, [[0, 1, 1], [1, 2, 1], [2, 3, 1], [0, 3, 1]], true },
            { 4, [[0, 1, 1], [2, 3, 1]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAConnectedGraph(int nodeCount, int[][] edges, bool expected)
        => Assert.Equal(expected, FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreePromise.IsKeptBy(nodeCount, edges));
}
