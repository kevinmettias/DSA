using DSAExperimentation.LeetCode.Solutions.FlowerPlantingWithNoAdjacent;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FlowerPlantingWithNoAdjacent;

// Harness only. Both strategies are FlowerPlantingWithNoAdjacentSolution's, and
// both walk the gardens in LeetCode's own 1..n order taking the lowest free flower
// type, so the answer is deterministic and can be stated outright rather than only
// checked for validity - the assertion helper checks both, since the planting
// property is what the problem actually asks for.
public sealed partial class FlowerPlantingWithNoAdjacentSolutionTests
{
    public static TheoryData<int, int[][], int[]> Cases =>
        new()
        {
            // A star: the center is planted first, so every leaf only has to
            // differ from it.
            { 4, [[1, 2], [1, 3], [1, 4]], [1, 2, 2, 2] },

            // No paths at all - every garden takes the first flower type.
            { 3, [], [1, 1, 1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GardenNoAdjByRawPathRescan_Cases_PlantsAFlowerTypeNoNeighborShares(
        int gardenCount, int[][] paths, int[] expected)
    {
        var flowers = FlowerPlantingWithNoAdjacentSolution.GardenNoAdjByRawPathRescan(gardenCount, paths);

        AssertValidPlanting(paths, expected, flowers);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void GardenNoAdjByAdjacencyList_Cases_PlantsAFlowerTypeNoNeighborShares(
        int gardenCount, int[][] paths, int[] expected)
    {
        var flowers = FlowerPlantingWithNoAdjacentSolution.GardenNoAdjByAdjacencyList(gardenCount, paths);

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
