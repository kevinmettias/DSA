using DSAExperimentation.LeetCode.Solutions.CountValidPathsInATree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountValidPathsInATree;

public sealed partial class CountValidPathsInATreePromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 5, [[1, 2], [1, 3], [2, 4], [2, 5]], true },
            { 1, [], true },
            { 4, [[1, 2], [2, 3]], false },
            { 3, [[1, 2], [2, 4]], false },
            { 4, [[1, 2], [2, 3], [3, 1]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheEdgesMakeATreeOfNodesOneToN(int nodeCount, int[][] edges, bool expected)
        => Assert.Equal(expected, CountValidPathsInATreePromise.IsKeptBy(nodeCount, edges));
}
