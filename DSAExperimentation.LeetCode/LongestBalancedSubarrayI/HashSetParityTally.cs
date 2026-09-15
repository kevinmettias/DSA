namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

// The brute-force arm's parity tally: one BCL HashSet<int> per parity, whose Add
// reports whether the value was new and whose Count is the distinct total the scan
// compares across the two parities. Deliberately written without this repo's
// primitives - it is the arm the composed strategy has to justify itself against.
internal sealed class HashSetParityTally : IParityTally<HashSet<int>>
{
    public static HashSetParityTally Instance { get; } = new();

    private HashSetParityTally()
    {
    }

    public HashSet<int> Fresh() => new();

    public int Count(HashSet<int> seen, int value)
    {
        seen.Add(value);

        return seen.Count;
    }
}
