namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 761 - how large an input to measure is a
// measurement decision, not a domain one.
internal static class SpecialBinaryStringWorkloads
{
    private const string OneBit = "1";
    private const string ZeroBit = "0";
    private const string MinimalSpecialString = "10";

    // Builds a valid special binary string containing exactly pairCount 1/0 pairs, by
    // recursively either wrapping a smaller special string ("1" + s + "0") or
    // concatenating two independently generated special strings - special strings are
    // closed under both operations, which is also exactly why
    // SpecialBinaryStringSolution's own recursion (splitting into top-level pieces,
    // then recursing on each piece's interior) is well-founded.
    public static string GenerateSpecial(int pairCount, Random random)
    {
        if (pairCount <= 1)
        {
            return MinimalSpecialString;
        }

        if (random.Next(2) == 0)
        {
            return OneBit + GenerateSpecial(pairCount - 1, random) + ZeroBit;
        }

        var left = random.Next(1, pairCount);
        return GenerateSpecial(left, random) + GenerateSpecial(pairCount - left, random);
    }
}
