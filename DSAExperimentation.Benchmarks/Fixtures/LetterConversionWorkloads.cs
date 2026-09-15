namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2976 - a 26-letter conversion ring
// (a -> b -> ... -> z -> a, each edge a random cost) so every character can
// reach every other one, keeping both strategies' per-position lookup on
// its full-cost path rather than short-circuiting on the first unreachable
// character; source and target are then independent random letters of the
// requested length.
internal static class LetterConversionWorkloads
{
    // The ring's own size and the exclusive bound on an edge cost - the two sizing
    // values this fixture is built out of, each typed once here rather than at every
    // place the array length, the wrap-around modulus and the random bound meet.
    private const int AlphabetSize = 26;
    private const int MaxEdgeCostExclusive = 1000;

    public static (char[] Original, char[] Changed, int[] Cost) BuildRing(int seed)
    {
        var random = new Random(seed);
        var original = new char[AlphabetSize];
        var changed = new char[AlphabetSize];
        var cost = new int[AlphabetSize];

        for (var i = 0; i < AlphabetSize; i++)
        {
            original[i] = (char)('a' + i);
            changed[i] = (char)('a' + ((i + 1) % AlphabetSize));
            cost[i] = random.Next(1, MaxEdgeCostExclusive);
        }

        return (original, changed, cost);
    }

    public static (string Source, string Target) BuildStrings(int length, int seed)
    {
        var random = new Random(seed);
        var source = new char[length];
        var target = new char[length];

        for (var i = 0; i < length; i++)
        {
            source[i] = (char)('a' + random.Next(AlphabetSize));
            target[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return (new string(source), new string(target));
    }
}
