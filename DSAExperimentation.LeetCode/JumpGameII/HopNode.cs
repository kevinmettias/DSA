namespace DSAExperimentation.LeetCode.JumpGameII;

// One index of LC 45's implicit reachability graph: wired with a weight-1 edge to
// every index reachable from it in a single jump. This is the problem's own input
// turned into a graph, not a general-purpose weighted-graph witness, so it lives
// beside the solution rather than in DataStructures/.
internal sealed record HopNode(int Index)
{
    public List<(int Weight, HopNode Target)> Edges { get; } = [];
}
