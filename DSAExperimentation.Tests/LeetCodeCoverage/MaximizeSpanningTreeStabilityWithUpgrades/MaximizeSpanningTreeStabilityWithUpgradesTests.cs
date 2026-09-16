using DSAExperimentation.LeetCode.MaximizeSpanningTreeStabilityWithUpgrades;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeSpanningTreeStabilityWithUpgrades;

// Harness only. Both strategies are
// MaximizeSpanningTreeStabilityWithUpgradesSolution's - this file just pins them
// to LeetCode's published examples, including the must-edge cycle that makes no
// spanning tree possible at all regardless of the upgrade budget.
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
        int nodeCount, int[][] edges, int upgrades, int expected)
    {
        var actual = MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByArrayUnionFind(
            nodeCount, edges, upgrades);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxStabilityByDisjointSet_LeetCodeExamples_ReturnsMaximumAchievableStability(
        int nodeCount, int[][] edges, int upgrades, int expected)
    {
        var actual = MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByDisjointSet(
            nodeCount, edges, upgrades);

        Assert.Equal(expected, actual);
    }
}
