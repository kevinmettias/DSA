using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.Tests.LeetCodeCoverage.FlowerPlantingWithNoAdjacent.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlowerPlantingWithNoAdjacent;

// LeetCode 1042. Flower Planting With No Adjacent: each garden has at most 3 paths
// (LeetCode's own constraint), so greedily assigning the first available flower
// type out of 4 always succeeds - no backtracking needed. Adjacency is this
// repo's own ListChildren/IGraphTopology pair (mirroring CourseSchedule's own
// Fixtures), and "which flower types are already taken by a neighbor" is this
// repo's own Set<int> rather than a hand-rolled bool[4].
public sealed partial class FlowerPlantingWithNoAdjacentTests
{
    [Fact]
    public void GardenNoAdj_Triangle_EveryPairOfConnectedGardensDiffers()
    {
        var gardens = BuildGardens(4, [[1, 2], [2, 3], [3, 1]]);

        var flowers = PlantFlowers(gardens);

        AssertNoAdjacentGardensShareAFlower(gardens, flowers);
    }

    [Fact]
    public void GardenNoAdj_StarShape_CenterDiffersFromEveryLeaf()
    {
        var gardens = BuildGardens(4, [[1, 2], [1, 3], [1, 4]]);

        var flowers = PlantFlowers(gardens);

        AssertNoAdjacentGardensShareAFlower(gardens, flowers);
    }

    [Fact]
    public void GardenNoAdj_NoPaths_EveryGardenGetsTheFirstFlowerType()
    {
        var gardens = BuildGardens(3, []);

        var flowers = PlantFlowers(gardens);

        Assert.All(flowers, flower => Assert.Equal(1, flower));
    }

    private static void AssertNoAdjacentGardensShareAFlower(List<GardenNode> gardens, int[] flowers)
    {
        foreach (var garden in gardens)
        {
            foreach (var neighbor in garden.ConnectedGardens)
            {
                Assert.NotEqual(flowers[garden.Id - 1], flowers[neighbor.Id - 1]);
            }
        }

        Assert.All(flowers, flower => Assert.InRange(flower, 1, 4));
    }

    private static List<GardenNode> BuildGardens(int gardenCount, int[][] paths)
    {
        var gardens = Enumerable.Range(1, gardenCount).Select(id => new GardenNode(id)).ToList();

        foreach (var path in paths)
        {
            var first = gardens[path[0] - 1];
            var second = gardens[path[1] - 1];
            first.ConnectedGardens.Add(second);
            second.ConnectedGardens.Add(first);
        }

        return gardens;
    }

    private static int[] PlantFlowers(List<GardenNode> gardens)
    {
        var flowers = new int[gardens.Count];

        foreach (var garden in gardens)
        {
            var usedFlowers = new Set<int>();
            var neighbors = GardenTopology.GetChildren(garden);

            for (var i = 0; i < neighbors.Count; i++)
            {
                usedFlowers.TryAdd(neighbors.Get(i).Flower);
            }

            for (var candidate = 1; candidate <= 4; candidate++)
            {
                if (!usedFlowers.Has(candidate))
                {
                    garden.Flower = candidate;
                    break;
                }
            }

            flowers[garden.Id - 1] = garden.Flower;
        }

        return flowers;
    }
}
