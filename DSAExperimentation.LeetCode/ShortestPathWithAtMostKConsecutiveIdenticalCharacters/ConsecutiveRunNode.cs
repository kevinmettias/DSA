namespace DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// One node per (physical graph node, length of the identical-label run ending
// there) state. Edges is wired once by ConsecutiveRunGraph.Build to every state
// the original edge set can reach without the run exceeding k - the same
// "node carries its own precomputed weighted edge list" shape RecoveryNode
// establishes for LC 3620.
internal sealed class ConsecutiveRunNode(int nodeId, int runLength)
{
    public int NodeId { get; } = nodeId;

    public int RunLength { get; } = runLength;

    public List<(long Weight, ConsecutiveRunNode Target)> Edges { get; } = [];

    public override string ToString() => $"{NodeId}:{RunLength}";
}
