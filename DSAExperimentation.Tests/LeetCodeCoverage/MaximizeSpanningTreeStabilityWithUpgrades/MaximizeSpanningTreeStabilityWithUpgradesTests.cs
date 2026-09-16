using DSAExperimentation.LeetCode.MaximizeSpanningTreeStabilityWithUpgrades;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeSpanningTreeStabilityWithUpgrades;

// Harness only. Both strategies are
// MaximizeSpanningTreeStabilityWithUpgradesSolution's - this file just pins them
// to LeetCode's published examples, including the must-edge cycle that makes no
// spanning tree possible at all regardless of k.
public sealed class MaximizeSpanningTreeStabilityWithUpgradesTests
{
    public static TheoryData<int, int[][], int, int> Examples =>
        new()
        {
            { 3, [[0, 1, 2, 1], [1, 2, 3, 0]], 1, 2 },
            { 3, [[0, 1, 4, 0], [1, 2, 3, 0], [0, 2, 1, 0]], 2, 6 },
            { 3, [[0, 1, 1, 1], [1, 2, 1, 1], [2, 0, 1, 1]], 0, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxStabilityByArrayUnionFind_LeetCodeExamples_ReturnsMaximumAchievableStability(
        int n, int[][] edges, int k, int expected)
    {
        var actual = MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByArrayUnionFind(n, edges, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxStabilityByDisjointSet_LeetCodeExamples_ReturnsMaximumAchievableStability(
        int n, int[][] edges, int k, int expected)
    {
        var actual = MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByDisjointSet(n, edges, k);

        Assert.Equal(expected, actual);
    }
}
