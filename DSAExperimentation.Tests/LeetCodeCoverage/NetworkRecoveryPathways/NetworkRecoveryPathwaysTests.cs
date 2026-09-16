using DSAExperimentation.LeetCode.NetworkRecoveryPathways;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NetworkRecoveryPathways;

// Harness only. RecoveryNetwork is NetworkRecoveryPathwaysSolution's own domain
// model and both search strategies are its methods - this file just pins them
// to LeetCode's published examples, plus one unreachable-destination case
// neither strategy's binary search should mistake for "threshold 0 is
// feasible."
public sealed class NetworkRecoveryPathwaysTests
{
    public static TheoryData<int[][], bool[], long, int> Examples =>
        new()
        {
            {
                [[0, 1, 5], [1, 3, 10], [0, 2, 3], [2, 3, 4]],
                [true, true, true, true],
                10L,
                3
            },
            {
                [[0, 1, 7], [1, 4, 5], [0, 2, 6], [2, 3, 6], [3, 4, 2], [2, 4, 6]],
                [true, true, true, false, true],
                12L,
                6
            },
            {
                [[0, 1, 5]],
                [true, true, true],
                100L,
                -1
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaxPathScoreByBruteForceDijkstra_LeetCodeExamples_ReturnsMaximumPathScore(
        int[][] edges, bool[] online, long k, int expected)
    {
        var actual = NetworkRecoveryPathwaysSolution.FindMaxPathScoreByBruteForceDijkstra(edges, online, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaxPathScoreByReduceGraph_LeetCodeExamples_ReturnsMaximumPathScore(
        int[][] edges, bool[] online, long k, int expected)
    {
        var actual = NetworkRecoveryPathwaysSolution.FindMaxPathScoreByReduceGraph(edges, online, k);

        Assert.Equal(expected, actual);
    }
}
