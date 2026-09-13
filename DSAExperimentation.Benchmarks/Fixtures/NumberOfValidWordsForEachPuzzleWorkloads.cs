namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1178 - the mask encoding itself lives in the
// solution; what stays here is only how many words and puzzles to generate and how
// wide their letter sets are, which is a measurement decision.
//
// Every generated word and puzzle has *distinct* letters, matching LC 1178's own
// constraint, and each puzzle is exactly 7 letters long so the composed strategy's
// submask enumeration does its full 2^7 work per puzzle.
internal static class NumberOfValidWordsForEachPuzzleWorkloads
{
    private const int MinWordLength = 3;
    private const int WordLengthUpperBound = 9; // exclusive; word length ranges [3, 8]
    private const int PuzzleLength = 7; // LC 1178: every puzzle has exactly 7 distinct letters
    private const int AlphabetSize = 26;

    public static string[] BuildWords(int count, Random random) =>
        Build(count, () => RandomLetters(random, random.Next(MinWordLength, WordLengthUpperBound)));

    public static string[] BuildPuzzles(int count, Random random) =>
        Build(count, () => RandomLetters(random, PuzzleLength));

    private static string[] Build(int count, Func<string> next)
    {
        var values = new string[count];

        for (var i = 0; i < count; i++)
        {
            values[i] = next();
        }

        return values;
    }

    private static string RandomLetters(Random random, int length)
    {
        var letters = new HashSet<char>();

        while (letters.Count < length)
        {
            letters.Add((char)('a' + random.Next(0, AlphabetSize)));
        }

        return new string([.. letters]);
    }
}
