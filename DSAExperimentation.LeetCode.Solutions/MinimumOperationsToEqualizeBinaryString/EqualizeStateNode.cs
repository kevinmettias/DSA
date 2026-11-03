namespace DSAExperimentation.LeetCode.Solutions.MinimumOperationsToEqualizeBinaryString;

// One node per reachable zero-count 0..n; Neighbors holds every zero-count one
// flip-exactly-k operation can reach from here, filled in once while the graph is
// built. Single-problem witness (LC 3666 alone), kept beside its solution; tier 1's
// StateNode has the same shape, built for any state space.
internal sealed class EqualizeStateNode(int zeroCount)
{
    public int ZeroCount { get; } = zeroCount;

    public List<EqualizeStateNode> Neighbors { get; } = [];

    public override string ToString() => ZeroCount.ToString();
}
