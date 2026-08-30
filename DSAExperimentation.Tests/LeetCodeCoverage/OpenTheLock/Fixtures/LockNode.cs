namespace DSAExperimentation.Tests.LeetCodeCoverage.OpenTheLock.Fixtures;

// One node per non-deadend 4-digit combination; Neighbors holds the (at most) 8
// combinations exactly one wheel-turn away, filled in once while the graph is
// built.
internal sealed class LockNode(string combination)
{
    public string Combination { get; } = combination;

    public List<LockNode> Neighbors { get; } = [];

    public override string ToString() => Combination;
}
