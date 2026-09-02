namespace DSAExperimentation.Domain.Locks;

// The geometry of a 4-wheel rotary lock: how many wheels it has, how far each one
// turns, and therefore how many combinations exist. Separate from LockGraph
// because these are the lock's dimensions, not the graph's - a caller sizing a
// workload over the combination space needs them without needing a built graph.
internal static class LockWheels
{
    // One wheel per digit position in a combination.
    public const int Count = 4;

    // Each wheel is a digit 0-9 turned with mod-10 wraparound arithmetic.
    public const int Modulus = 10;

    // Every combination: Modulus ^ Count.
    public const int CombinationSpace = 10_000;

    // Combination-to-string format: 4 digits, zero-padded.
    private const string CombinationFormat = "D4";

    public static string Combination(int index) => index.ToString(CombinationFormat);
}
