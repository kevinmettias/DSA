using DSAExperimentation.LeetCode.Solutions.CollectCoinsInATree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CollectCoinsInATree;

public sealed partial class CollectCoinsInATreePromiseTests
{
    public static TheoryData<int[], int[][], bool> Inputs =>
        new()
        {
            { [1, 0, 0, 0, 0, 1], [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5]], true },
            { [1], [], true },
            { [1, 0], [[0, 2]], false },
            { [1, 0, 0], [[0, 1], [1, 0]], false },
            { [1, 0, 0], [[0, 1]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheEdgesMakeATreeOfOneNodePerCoin(int[] coins, int[][] edges, bool expected)
        => Assert.Equal(expected, CollectCoinsInATreePromise.IsKeptBy(coins, edges));
}
