using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumIncompatibility;

// This repo's own Set<int> plays the same "reject a repeated value" role HashSet.Add
// does (TryAdd's newly-added bool is exactly its return contract), on the arm that is
// composed from this library.
internal sealed class SetNovelty : INoveltySet<Set<int>>
{
    public static SetNovelty Instance { get; } = new();

    private SetNovelty()
    {
    }

    public Set<int> Fresh() => new();

    public bool TryAdmit(Set<int> seen, int value) => seen.TryAdd(value);
}
