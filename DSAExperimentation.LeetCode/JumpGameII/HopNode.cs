namespace DSAExperimentation.LeetCode.JumpGameII;

// One index of LC 45's implicit reachability graph: wired with an edge to every index
// reachable from it in a single jump. Every jump is one step, so the edges carry no weight.
// This is the problem's own input turned into a graph, not a general-purpose graph witness,
// so it lives beside the solution rather than in DataStructures/.
internal sealed record HopNode(int Index)
{
    public List<HopNode> Targets { get; } = [];
}
