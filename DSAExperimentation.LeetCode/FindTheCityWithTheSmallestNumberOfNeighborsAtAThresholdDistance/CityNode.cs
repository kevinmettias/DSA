namespace DSAExperimentation.LeetCode.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// One node per city; Edges holds both directions of every road, since LC 1334's
// roads are undirected. Answers LC 1334 alone - a fully generic weighted node
// belongs in DataStructures, but nothing that generic exists in
// DSAExperimentation yet (EdgeGraphNode's own doc comment makes the same call
// for LC 3123's identical shape), so this stays a problem-local witness rather
// than a sixth copy of the harness-tier WeightedNode/WeightedGraphNode pair the
// pre-migration test and benchmark each reached for.
internal sealed class CityNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, CityNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
