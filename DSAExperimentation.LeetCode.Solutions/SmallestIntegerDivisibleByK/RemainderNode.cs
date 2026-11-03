namespace DSAExperimentation.LeetCode.Solutions.SmallestIntegerDivisibleByK;

// One node per remainder mod the divisor. Appending another '1' digit to a repunit with
// remainder r produces remainder (r*10+1) % divisor - a single deterministic successor,
// so Neighbors always holds exactly one entry, filled in once while the graph is
// built: tier 1's StateNode shape, with the one successor where a lock's node has eight.
//
// "Edge = append one decimal '1'" is LC 1015's own content rather than a reusable
// shape, so this node lives beside the solution instead of in DataStructures or
// at the tier's root (ARCHITECTURE.md §17.3).
internal sealed class RemainderNode(int remainder)
{
    public int Remainder { get; } = remainder;

    public List<RemainderNode> Neighbors { get; } = [];

    public override string ToString() => Remainder.ToString();
}
