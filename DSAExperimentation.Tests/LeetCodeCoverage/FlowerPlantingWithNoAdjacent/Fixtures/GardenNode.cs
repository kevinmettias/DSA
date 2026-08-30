namespace DSAExperimentation.Tests.LeetCodeCoverage.FlowerPlantingWithNoAdjacent.Fixtures;

// One-indexed to match LeetCode's own 1..n garden ids and paths input. Flower is 0
// until PlantFlowers assigns it, the same "unset" sentinel HouseRobber-style
// int fields use elsewhere in this repo's coverage tests.
internal sealed class GardenNode(int id)
{
    public int Id { get; } = id;

    public List<GardenNode> ConnectedGardens { get; } = [];

    public int Flower { get; set; }
}
