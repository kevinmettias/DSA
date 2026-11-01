namespace DSAExperimentation.Domain.SlidingPuzzle;

// One node per reachable 2x3 board permutation ("012345"'s 720 arrangements, '0'
// standing in for the empty slot); Neighbors holds the (at most 3) boards exactly
// one blank-tile slide away, filled in once while the graph is built - the same
// role OpenTheLock's LockNode.Neighbors plays for wheel-turn adjacency.
/// <summary>One board of the 2x3 sliding puzzle and the boards one blank-tile slide away.</summary>
/// <param name="state">The board read row by row, <c>'0'</c> standing for the blank.</param>
public sealed class PuzzleNode(string state)
{
    /// <summary>The board read row by row, <c>'0'</c> standing for the blank.</summary>
    public string State { get; } = state;

    /// <summary>The boards one blank-tile slide away, two or three, filled in while the graph is built.</summary>
    public List<PuzzleNode> Neighbors { get; } = [];

    public override string ToString() => State;
}
