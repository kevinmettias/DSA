using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

// The composed arm's parity tally: this repo's own Set<int>, whose TryAdd plays
// HashSet.Add's part (a newly-added bool) and whose Count is the same distinct total -
// so the scan above it is one body for both arms, with only the structure behind a
// parity's tally differing.
internal sealed class SetParityTally : IParityTally<Set<int>>
{
    public static SetParityTally Instance { get; } = new();

    private SetParityTally()
    {
    }

    public Set<int> Fresh() => new();

    public int Count(Set<int> seen, int value)
    {
        seen.TryAdd(value);

        return seen.Count;
    }
}
