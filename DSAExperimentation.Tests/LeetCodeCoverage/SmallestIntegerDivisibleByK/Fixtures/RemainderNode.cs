namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestIntegerDivisibleByK.Fixtures;

// One node per remainder mod k. Appending another '1' digit to a repunit with
// remainder r produces remainder (r*10+1) % k - a single deterministic successor,
// so Neighbors always holds exactly one entry, filled in once while the graph is
// built (LockNode precedent, minus the "up to 8" branching factor).
internal sealed class RemainderNode(int remainder)
{
    public int Remainder { get; } = remainder;

    public List<RemainderNode> Neighbors { get; } = [];

    public override string ToString() => Remainder.ToString();
}
