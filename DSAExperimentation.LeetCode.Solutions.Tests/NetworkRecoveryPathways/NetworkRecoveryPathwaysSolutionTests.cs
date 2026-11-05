using DSAExperimentation.LeetCode.Solutions.NetworkRecoveryPathways;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NetworkRecoveryPathways;

// Harness only. RecoveryNetwork is NetworkRecoveryPathwaysSolution's own domain
// model and both search strategies are its methods - this file just pins them
// to one unreachable-destination case
// neither strategy's binary search should mistake for "threshold 0 is
// feasible."
public sealed partial class NetworkRecoveryPathwaysSolutionTests
{
    public static TheoryData<int[][], bool[], long, int> Cases =>
        new()
        {
            {
                [[0, 1, 5]],
                [true, true, true],
                100L,
                -1
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaxPathScoreByBruteForceDijkstra_Cases_ReturnsMaximumPathScore(
        int[][] edges, bool[] online, long costBudget, int expected)
    {
        var actual = NetworkRecoveryPathwaysSolution.FindMaxPathScoreByBruteForceDijkstra(
            edges, online, costBudget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaxPathScoreByReduceGraph_Cases_ReturnsMaximumPathScore(
        int[][] edges, bool[] online, long costBudget, int expected)
    {
        var actual = NetworkRecoveryPathwaysSolution.FindMaxPathScoreByReduceGraph(
            edges, online, costBudget);

        Assert.Equal(expected, actual);
    }
}
