namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' SlidingPuzzle PuzzleNode fixture: one node per
// reachable 2x3 board permutation, Neighbors holding the (at most 3) boards exactly
// one blank-tile slide away.
internal sealed class PuzzleNode(string state)
{
    public string State { get; } = state;

    public List<PuzzleNode> Neighbors { get; } = [];
}
