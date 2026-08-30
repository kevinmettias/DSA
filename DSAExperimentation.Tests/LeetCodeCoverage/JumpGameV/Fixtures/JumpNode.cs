namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameV.Fixtures;

// Edges point i -> j only for indices actually reachable by a single valid jump
// (arr[i] strictly greater than arr[j] and every value strictly between them) -
// every edge therefore drops in value, which is exactly what rules out cycles and
// lets TopologicalSort.TrySort always succeed on this graph.
internal sealed class JumpNode(int index)
{
    public int Index { get; } = index;

    public List<JumpNode> ReachableIndices { get; } = [];

    public override string ToString() => Index.ToString();
}
