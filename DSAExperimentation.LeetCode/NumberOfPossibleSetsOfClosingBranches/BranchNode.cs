namespace DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

// One node per branch; Edges holds every road out of it, both directions of every
// undirected road in the input. Answers LC 2959 alone - a fully generic weighted
// node belongs in DataStructures, not here, but nothing that generic exists in
// DSAExperimentation yet (the repo's only other weighted-node fixture,
// Benchmarks.Fixtures.WeightedGraphNode, lives in a project this solution cannot
// reference), so this stays a problem-local witness the way RoomWaysAlgebra does.
internal sealed class BranchNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, BranchNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
