using DSAExperimentation.LeetCode.Solutions.RedundantConnectionII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RedundantConnectionII;

public sealed partial class RedundantConnectionIIPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 2], [1, 3], [2, 3]], true },
            { [[1, 2], [2, 3], [3, 4], [4, 1], [1, 5]], true },
            { [[3, 1], [4, 1], [5, 2], [3, 2], [3, 5]], false },
            { [[1, 2], [2, 9], [1, 3]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForARootedTreeWithOneEdgeAdded(int[][] edges, bool expected)
        => Assert.Equal(expected, RedundantConnectionIIPromise.IsKeptBy(edges));
}
