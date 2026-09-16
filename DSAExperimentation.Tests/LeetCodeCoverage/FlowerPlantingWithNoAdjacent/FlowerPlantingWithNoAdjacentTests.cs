using DSAExperimentation.LeetCode.FlowerPlantingWithNoAdjacent;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlowerPlantingWithNoAdjacent;

// Harness only. Both strategies are FlowerPlantingWithNoAdjacentSolution's, and
// both walk the gardens in LeetCode's own 1..n order taking the lowest free flower
// type, so the answer is deterministic and can be stated outright rather than only
// checked for validity - the assertion helper checks both, since the planting
// property is what the problem actually asks for.
public sealed class FlowerPlantingWithNoAdjacentTests
{
    public static TheoryData<int, int[][], int[]> Examples =>
        new()
        {
            // LeetCode's own three examples.
            { 3, [[1, 2], [2, 3], [3, 1]], [1, 2, 3] },
            { 4, [[1, 2], [3, 4]], [1, 2, 1, 2] },
            { 4, [[1, 2], [2, 3], [3, 4], [4, 1], [1, 3], [2, 4]], [1, 2, 3, 4] },

            // A star: the center is planted first, so every leaf only has to
            // differ from it.
            { 4, [[1, 2], [1, 3], [1, 4]], [1, 2, 2, 2] },

            // No paths at all - every garden takes the first flower type.
            { 3, [], [1, 1, 1] },

            // A chain, where each garden sees exactly one already-planted
            // neighbor.
            { 5, [[1, 2], [2, 3], [3, 4], [4, 5]], [1, 2, 1, 2, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GardenNoAdjByRawPathRescan_LeetCodeExamples_PlantsAFlowerTypeNoNeighborShares(
        int n, int[][] paths, int[] expected)
    {
        var flowers = FlowerPlantingWithNoAdjacentSolution.GardenNoAdjByRawPathRescan(n, paths);

        AssertValidPlanting(paths, expected, flowers);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GardenNoAdjByAdjacencyList_LeetCodeExamples_PlantsAFlowerTypeNoNeighborShares(
        int n, int[][] paths, int[] expected)
    {
        var flowers = FlowerPlantingWithNoAdjacentSolution.GardenNoAdjByAdjacencyList(n, paths);

        AssertValidPlanting(paths, expected, flowers);
    }

    private static void AssertValidPlanting(int[][] paths, int[] expected, int[] flowers)
    {
        Assert.Equal(expected, flowers);
        Assert.All(flowers, flower => Assert.InRange(flower, 1, 4));

        foreach (var path in paths)
        {
            Assert.NotEqual(flowers[path[0] - 1], flowers[path[1] - 1]);
        }
    }
}
