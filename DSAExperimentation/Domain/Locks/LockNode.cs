namespace DSAExperimentation.Domain.Locks;

// One node per reachable 4-digit combination of a rotary combination lock;
// Neighbors holds the (at most) 8 combinations exactly one wheel-turn away,
// filled in once while the graph is built.
internal sealed class LockNode(string combination)
{
    public string Combination { get; } = combination;

    public List<LockNode> Neighbors { get; } = [];

    public override string ToString() => Combination;
}
