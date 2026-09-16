namespace DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

// The full n-branch road network, built once - the domain model, not an answer to
// any one query about it, the same LockGraph split between "what the puzzle looks
// like" and "what a caller wants to find out" (Domain.Locks.LockGraph's own
// framing). Multiple roads between the same two branches are kept as separate
// edges rather than pre-reduced to their minimum - AllPairsShortestPaths already
// takes the shorter one on relax, so there is nothing to precompute here.
internal sealed class BranchNetwork
{
    public BranchNode[] Nodes { get; }

    private BranchNetwork(BranchNode[] nodes) => Nodes = nodes;

    // A BranchNode per branch id, then both directions of every road. LeetCodeAdjacency
    // states that layout once for every problem taking an (n, edges) pair; this
    // problem's own detail is that a slot holds the road's weight beside its far
    // branch, which is what the wiring callback reads the road's third value for.
    public static BranchNetwork Build(int n, int[][] roads)
    {
        var nodes = LeetCodeAdjacency.ZeroBased<BranchNode>(
            n, roads, id => new BranchNode(id), (node, _, farNode, edgeIndex) => node.Edges.Add((roads[edgeIndex][2], farNode)));

        return new BranchNetwork(nodes);
    }
}
