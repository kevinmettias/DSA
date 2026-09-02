namespace DSAExperimentation.LeetCode.MinimizeMaximumComponentCost;

// One node per LeetCode vertex id (0..n-1). Shaped like Domain.Locks' LockNode -
// an identity plus a mutable adjacency list wired in after construction - but
// stays local to this problem: nothing else in the catalogue needs a bare
// weighted-adjacency node yet.
internal sealed class ComponentNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, ComponentNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
