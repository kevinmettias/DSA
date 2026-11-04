using DSAExperimentation.LeetCode.Solutions.MaximumPointsAfterCollectingCoinsFromAllNodes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumPointsAfterCollectingCoinsFromAllNodes;

public sealed partial class MaximumPointsAfterCollectingCoinsFromAllNodesPromiseTests
{
    public static TheoryData<int[][], int[], int, bool> Inputs =>
        new()
        {
            { [[0, 1], [1, 2], [2, 3]], [10, 10, 3, 3], 5, true },
            { [[0, 1], [0, 2]], [8, 4, 4], 0, true },
            { [[0, 1]], [1, 2, 3], 0, false },
            { [[0, 1], [1, 0]], [1, 2, 3], 0, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheEdgesFormOneTree(int[][] edges, int[] coins, int cost, bool expected)
        => Assert.Equal(expected, MaximumPointsAfterCollectingCoinsFromAllNodesPromise.IsKeptBy(edges, coins, cost));
}
