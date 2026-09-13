namespace DSAExperimentation.LeetCode.FlowerPlantingWithNoAdjacent;

// One garden in LC 1042's path graph, with ConnectedGardens holding every garden
// reachable by a direct path. Paths are bidirectional, so every listed pair is
// wired both ways. Answers LC 1042 alone - a plain adjacency-list graph node with
// no fixed vertex set or modulus, so it lives beside the solution rather than in
// Domain/ (ARCHITECTURE.md #17.6), exactly like CourseSchedule's CourseNode.
internal sealed class GardenNode(int id)
{
    public int Id { get; } = id;

    public List<GardenNode> ConnectedGardens { get; } = [];

    public override string ToString() => Id.ToString();
}
