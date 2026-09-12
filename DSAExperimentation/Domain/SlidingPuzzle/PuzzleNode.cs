namespace DSAExperimentation.Domain.SlidingPuzzle;

// One node per reachable 2x3 board permutation ("012345"'s 720 arrangements, '0'
// standing in for the empty slot); Neighbors holds the (at most 3) boards exactly
// one blank-tile slide away, filled in once while the graph is built - the same
// role Domain.Locks' LockNode.Neighbors plays for wheel-turn adjacency.
internal sealed class PuzzleNode(string state)
{
    public string State { get; } = state;

    public List<PuzzleNode> Neighbors { get; } = [];

    public override string ToString() => State;
}
