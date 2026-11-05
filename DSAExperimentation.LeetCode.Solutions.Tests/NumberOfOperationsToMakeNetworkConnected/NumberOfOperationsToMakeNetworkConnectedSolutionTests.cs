using DSAExperimentation.LeetCode.Solutions.NumberOfOperationsToMakeNetworkConnected;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfOperationsToMakeNetworkConnected;

// Harness only. Both component counts - the DFS flood fill and the DisjointSet
// walk - are NumberOfOperationsToMakeNetworkConnectedSolution's; this file just
// pins them to the cases the two arms are most
// likely to disagree on: too few cables to connect anything, a network that is
// already connected, and a single computer with no cables at all.
public sealed partial class NumberOfOperationsToMakeNetworkConnectedSolutionTests
{
    public static TheoryData<int, int[][], int> Cases =>
        new()
        {
            { 4, [[0, 1], [1, 2], [2, 3]], 0 },
            { 5, [[0, 1], [1, 2], [0, 2], [3, 4]], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MakeConnectedByDepthFirstFloodFill_Cases_ReturnsCablesThatMustMove(
        int computerCount, int[][] connections, int expected)
    {
        var actual = NumberOfOperationsToMakeNetworkConnectedSolution.MakeConnectedByDepthFirstFloodFill(computerCount, connections);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MakeConnectedByDisjointSet_Cases_ReturnsCablesThatMustMove(
        int computerCount, int[][] connections, int expected)
    {
        var actual = NumberOfOperationsToMakeNetworkConnectedSolution.MakeConnectedByDisjointSet(computerCount, connections);

        Assert.Equal(expected, actual);
    }
}
