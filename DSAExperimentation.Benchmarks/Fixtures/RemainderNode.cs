namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' SmallestIntegerDivisibleByK RemainderNode
// fixture: one node per remainder mod k, Neighbors holding the single remainder
// reached by appending one more '1' digit.
internal sealed class RemainderNode(int remainder)
{
    public int Remainder { get; } = remainder;

    public List<RemainderNode> Neighbors { get; } = [];
}
