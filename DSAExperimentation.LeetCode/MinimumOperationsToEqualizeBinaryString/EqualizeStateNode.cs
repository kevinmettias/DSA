namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeBinaryString;

// One node per reachable zero-count 0..n; Neighbors holds every zero-count one
// flip-exactly-k operation can reach from here, filled in once while the graph is
// built. Single-problem witness (LC 3666 alone), the same shape as
// Domain.Locks.LockNode one tier down for a problem shared by several puzzles.
internal sealed class EqualizeStateNode(int zeroCount)
{
    public int ZeroCount { get; } = zeroCount;

    public List<EqualizeStateNode> Neighbors { get; } = [];

    public override string ToString() => ZeroCount.ToString();
}
