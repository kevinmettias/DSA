namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2977 - a handful of short (length 1-3)
// substring rules, plus independent random source/target strings of the
// requested length, so both strategies' per-position window scan has real
// candidate substrings to test rather than missing on every lookup.
internal static class SubstringConversionWorkloads
{
    private const int RuleCount = 60;

    public static (string[] Original, string[] Changed, int[] Cost) BuildRules(int seed)
    {
        var random = new Random(seed);
        var original = new string[RuleCount];
        var changed = new string[RuleCount];
        var cost = new int[RuleCount];

        for (var i = 0; i < RuleCount; i++)
        {
            var length = 1 + (i % 3);
            original[i] = RandomString(random, length);
            changed[i] = RandomString(random, length);
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

    private static string RandomString(Random random, int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(26));
        }

        return new string(chars);
    }
}
