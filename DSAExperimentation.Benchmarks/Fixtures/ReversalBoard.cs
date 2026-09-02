using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' MinimumReverseOperations ReversalBoard fixture:
// the shared, read-only context a PositionNode carries a reference to.
internal sealed class ReversalBoard(int length, int k, Set<int> banned)
{
    public int Length { get; } = length;

    public int K { get; } = k;

    public bool IsBanned(int position) => banned.Has(position);
}
