using DSAExperimentation.LeetCode.NumberOfOperationsToMakeNetworkConnected;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfOperationsToMakeNetworkConnected;

// Harness only. Both component counts - the DFS flood fill and the DisjointSet
// walk - are NumberOfOperationsToMakeNetworkConnectedSolution's; this file just
// pins them to LeetCode's published examples plus the cases the two arms are most
// likely to disagree on: too few cables to connect anything, a network that is
// already connected, and a single computer with no cables at all.
public sealed partial class NumberOfOperationsToMakeNetworkConnectedTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            { 4, [[0, 1], [0, 2], [1, 2]], 1 },
            { 6, [[0, 1], [0, 2], [0, 3], [1, 2], [1, 3]], 2 },
            { 6, [[0, 1], [0, 2], [0, 3], [1, 2]], -1 },
            { 4, [[0, 1], [1, 2], [2, 3]], 0 },
            { 5, [[0, 1], [1, 2], [0, 2], [3, 4]], 1 },
            { 1, [], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeConnectedByDepthFirstFloodFill_LeetCodeExamples_ReturnsCablesThatMustMove(
        int computerCount, int[][] connections, int expected)
    {
        var actual = NumberOfOperationsToMakeNetworkConnectedSolution.MakeConnectedByDepthFirstFloodFill(computerCount, connections);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeConnectedByDisjointSet_LeetCodeExamples_ReturnsCablesThatMustMove(
        int computerCount, int[][] connections, int expected)
    {
        var actual = NumberOfOperationsToMakeNetworkConnectedSolution.MakeConnectedByDisjointSet(computerCount, connections);

        Assert.Equal(expected, actual);
    }
}
