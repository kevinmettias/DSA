namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3455 - a random string over a 6-letter alphabet
// small enough that "ab", "cd" and "ef" (the fixed pattern's literal parts) each
// turn up many times, so the search actually has candidates to compare instead of
// falling through to -1 on every run. Six letters is the floor, not a taste call:
// dropping to four would leave "ef" unable to occur at all, which is the -1 fast
// path this fixture exists to keep the benchmark off.
internal static class ShortestMatchingSubstringWorkloads
{
    private const string Alphabet = "abcdef";

    public static string BuildText(int length, int seed)
    {
        var random = new Random(seed);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(chars);
    }
}
