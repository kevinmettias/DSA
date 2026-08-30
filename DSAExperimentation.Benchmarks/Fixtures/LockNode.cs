namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' OpenTheLock LockNode fixture: one node per
// non-deadend 4-digit combination, Neighbors holding the (at most) 8 combinations
// exactly one wheel-turn away.
internal sealed class LockNode(string combination)
{
    public string Combination { get; } = combination;

    public List<LockNode> Neighbors { get; } = [];
}
