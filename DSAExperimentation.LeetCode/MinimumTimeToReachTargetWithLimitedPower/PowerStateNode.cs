namespace DSAExperimentation.LeetCode.MinimumTimeToReachTargetWithLimitedPower;

// One node per (physical graph node, remaining power) state. Edges is wired
// once by PowerStateGraph.Build to every state that spending cost[node] on an
// outgoing edge can reach - the same "node carries its own precomputed
// weighted edge list" shape RecoveryNode establishes for LC 3620, here with
// remaining power standing in for RecoveryNode's plain id.
internal sealed class PowerStateNode(int nodeId, int remainingPower)
{
    public int NodeId { get; } = nodeId;

    public int RemainingPower { get; } = remainingPower;

    public List<(long Weight, PowerStateNode Target)> Edges { get; } = [];

    public override string ToString() => $"{NodeId}@{RemainingPower}";
}
