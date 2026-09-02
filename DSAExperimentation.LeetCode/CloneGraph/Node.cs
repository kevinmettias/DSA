namespace DSAExperimentation.LeetCode.CloneGraph;

// LeetCode's own published input shape for LC 133: a value plus a mutable list
// of neighbor references, wired in after construction so a cyclic (undirected)
// graph can be built at all. Stays local to this problem - nothing else in the
// catalogue is handed a graph already fully formed, caller-owned and self-
// referential like this, rather than building one from a domain rule.
internal sealed class Node(int value)
{
    public int Value { get; } = value;

    public List<Node> Neighbors { get; } = [];
}
