namespace DSAExperimentation.LeetCode.Solutions.LoudAndRich;

// One node per person. Poorer holds everyone known to have strictly less money -
// the direction TopologicalSort.TrySort walks to process richest-to-poorest -
// and Richer holds the reverse edges the per-person baseline walk follows out of
// each person. Answers LC 851 alone: it holds both directions of every edge, where
// DataStructures' AdjacencyNode holds only the out-edges, so it lives beside the
// solution rather than in DataStructures/ (ARCHITECTURE.md #17.6).
internal sealed class PersonNode(int id)
{
    public int Id { get; } = id;

    public List<PersonNode> Poorer { get; } = [];

    public List<PersonNode> Richer { get; } = [];

    public override string ToString() => $"Person({Id})";
}
