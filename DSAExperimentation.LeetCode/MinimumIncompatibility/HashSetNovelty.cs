namespace DSAExperimentation.LeetCode.MinimumIncompatibility;

// The baseline arm's novelty set: BCL HashSet<int>, whose Add answers "has this value
// been seen already" and records it in the one lookup - so it is handed to the group
// walk as that question alone. Deliberately written without this repo's primitives,
// as a baseline is meant to be.
internal sealed class HashSetNovelty : INoveltySet<HashSet<int>>
{
    public static HashSetNovelty Instance { get; } = new();

    private HashSetNovelty()
    {
    }

    public HashSet<int> Fresh() => new();

    public bool TryAdmit(HashSet<int> seen, int value) => seen.Add(value);
}
