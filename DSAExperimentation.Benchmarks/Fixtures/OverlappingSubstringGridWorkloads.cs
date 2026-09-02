namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3529 - a low-cardinality alphabet ('a'/'b')
// so a naive scan spends real comparison work on near-misses instead of
// rejecting most starting positions after a single character, the scenario a
// linear-time string search actually earns its keep on.
internal static class OverlappingSubstringGridWorkloads
{
    private const int PatternLength = 6;
    private const string Alphabet = "ab";

    public static (char[][] Grid, string Pattern) Build(int size, int seed)
    {
        var random = new Random(seed);
        var grid = new char[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new char[size];

            for (var col = 0; col < size; col++)
            {
                grid[row][col] = Alphabet[random.Next(Alphabet.Length)];
            }
        }

        var pattern = new char[PatternLength];

        for (var i = 0; i < PatternLength; i++)
        {
            pattern[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return (grid, new string(pattern));
    }
}
