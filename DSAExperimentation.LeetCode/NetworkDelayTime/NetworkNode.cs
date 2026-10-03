namespace DSAExperimentation.LeetCode.NetworkDelayTime;

// LC 743's own directed, non-negative-weight edge list. ShortestPathAlgorithmBenchmarks
// builds its random graphs from this type too, so the benchmark walks the node and
// topology the solution's tests walk rather than a harness-tier copy of them.
internal sealed record NetworkNode(int Id)
{
    public List<(int Weight, NetworkNode Target)> Edges { get; } = [];
}
