namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2976 - a 26-letter conversion ring
// (a -> b -> ... -> z -> a, each edge a random cost) so every character can
// reach every other one, keeping both strategies' per-position lookup on
// its full-cost path rather than short-circuiting on the first unreachable
// character; source and target are then independent random letters of the
// requested length.
internal static class LetterConversionWorkloads
{
    public static (char[] Original, char[] Changed, int[] Cost) BuildRing(int seed)
    {
        var random = new Random(seed);
        var original = new char[26];
        var changed = new char[26];
        var cost = new int[26];

        for (var i = 0; i < 26; i++)
        {
            original[i] = (char)('a' + i);
            changed[i] = (char)('a' + ((i + 1) % 26));
            cost[i] = random.Next(1, 1000);
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
            source[i] = (char)('a' + random.Next(26));
            target[i] = (char)('a' + random.Next(26));
        }

        return (new string(source), new string(target));
    }
}
