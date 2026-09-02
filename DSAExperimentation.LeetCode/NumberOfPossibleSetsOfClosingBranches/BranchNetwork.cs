namespace DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

// The full n-branch road network, built once - the domain model, not an answer to
// any one query about it, the same LockGraph split between "what the puzzle looks
// like" and "what a caller wants to find out" (Domain.Locks.LockGraph's own
// framing). Multiple roads between the same two branches are kept as separate
// edges rather than pre-reduced to their minimum - AllPairsShortestPaths already
// takes the shorter one on relax, so there is nothing to precompute here.
internal sealed class BranchNetwork
{
    private BranchNetwork(BranchNode[] nodes) => Nodes = nodes;

    public BranchNode[] Nodes { get; }

    public static BranchNetwork Build(int n, int[][] roads)
    {
        var nodes = new BranchNode[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new BranchNode(i);
        }

        foreach (var road in roads)
        {
            var (from, to, weight) = (road[0], road[1], road[2]);
            nodes[from].Edges.Add((weight, nodes[to]));
            nodes[to].Edges.Add((weight, nodes[from]));
        }

        return new BranchNetwork(nodes);
    }
}
