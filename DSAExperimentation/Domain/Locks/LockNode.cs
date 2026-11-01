namespace DSAExperimentation.Domain.Locks;

// One node per reachable 4-digit combination of a rotary combination lock;
// Neighbors holds the (at most) 8 combinations exactly one wheel-turn away,
// filled in once while the graph is built.
/// <summary>One live combination of a rotary lock and the live combinations one wheel-turn away.</summary>
/// <param name="combination">The four-digit combination this node stands for.</param>
public sealed class LockNode(string combination)
{
    /// <summary>The four-digit combination this node stands for.</summary>
    public string Combination { get; } = combination;

    /// <summary>The live combinations one wheel-turn away, at most eight, filled in while the graph is built.</summary>
    public List<LockNode> Neighbors { get; } = [];

    public override string ToString() => Combination;
}
