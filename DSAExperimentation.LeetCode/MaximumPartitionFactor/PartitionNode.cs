namespace DSAExperimentation.LeetCode.MaximumPartitionFactor;

// One node per input point. Neighbors holds every OTHER point currently closer
// than the Manhattan-distance threshold under test - "too close to share a
// group" - rebuilt fresh for each binary-search candidate by
// MaximumPartitionFactorSolution.MaxPartitionFactorByBinarySearchBipartiteCheck,
// the same "mutable Neighbors filled in as the graph is (re)built" shape
// Domain.Locks.LockNode already uses.
internal sealed class PartitionNode(int x, int y)
{
    public int X { get; } = x;

    public int Y { get; } = y;

    public List<PartitionNode> Neighbors { get; } = [];
}
