namespace DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

// One node per remainder mod k. Appending another '1' digit to a repunit with
// remainder r produces remainder (r*10+1) % k - a single deterministic successor,
// so Neighbors always holds exactly one entry, filled in once while the graph is
// built (LockNode precedent, minus the "up to 8" branching factor).
//
// "Edge = append one decimal '1'" is LC 1015's own content rather than a reusable
// shape, so this node lives beside the solution instead of in DataStructures or
// Domain (ARCHITECTURE.md §17.3).
internal sealed class RemainderNode(int remainder)
{
    public int Remainder { get; } = remainder;

    public List<RemainderNode> Neighbors { get; } = [];

    public override string ToString() => Remainder.ToString();
}
