namespace DSAExperimentation.LeetCode.LongestCycleInAGraph;

// LC 2360's encoding of its own input, and nothing else: edges[i] names node i's
// single successor, except for this one value, which says node i has no outgoing
// edge at all. Both the graph builder and the raw forward walk have to read that
// encoding, so it is declared once here rather than copied into each - a type
// whose subject IS this value, the same shape Domain.Locks.LockWheels has.
internal static class FunctionalGraphEdges
{
    public const int NoOutgoingEdge = -1;
}
