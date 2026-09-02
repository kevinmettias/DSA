namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3455 - a random string over a 4-letter alphabet
// small enough that "ab", "cd" and "ef" (the fixed pattern's literal parts) each
// turn up many times, so the search actually has candidates to compare instead of
// falling through to -1 on every run.
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
