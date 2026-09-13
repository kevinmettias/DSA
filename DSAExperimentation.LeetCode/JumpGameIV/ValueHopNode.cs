namespace DSAExperimentation.LeetCode.JumpGameIV;

// One index of LC 1345's implicit reachability graph: wired with a weight-1 edge
// to i+1, to i-1, and to every other index holding the same value. This is the
// problem's own input turned into a graph, not a general-purpose weighted-graph
// witness, so - like JumpGameII's HopNode - it lives beside the solution rather
// than in DataStructures/.
internal sealed class ValueHopNode(int index)
{
    public int Index { get; } = index;

    public List<(int Weight, ValueHopNode Target)> Edges { get; } = [];

    public override string ToString() => Index.ToString();
}
