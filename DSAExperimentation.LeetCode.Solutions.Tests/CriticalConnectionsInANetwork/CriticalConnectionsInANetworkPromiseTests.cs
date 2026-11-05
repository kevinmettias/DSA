using DSAExperimentation.LeetCode.Solutions.CriticalConnectionsInANetwork;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CriticalConnectionsInANetwork;

public sealed partial class CriticalConnectionsInANetworkPromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 4, [[0, 1], [1, 2], [2, 0], [1, 3]], true },
            { 2, [[0, 1]], true },
            { 4, [[0, 1], [2, 3]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAConnectedNetwork(int serverCount, int[][] connections, bool expected)
        => Assert.Equal(expected, CriticalConnectionsInANetworkPromise.IsKeptBy(serverCount, connections));
}
