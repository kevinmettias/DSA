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

    public static string[] BuildWords(int count, Random random) => Build(count, new WordSource(random));

    public static string[] BuildPuzzles(int count, Random random) => Build(count, new PuzzleSource(random));

    private static string RandomLetters(Random random, int length)
    {
        var letters = new HashSet<char>();

        while (letters.Count < length)
        {
            letters.Add((char)('a' + random.Next(0, AlphabetSize)));
        }

        return new string([.. letters]);
    }

    private static string[] Build(int count, IWorkloadEntrySource source)
    {
        var values = new string[count];

        for (var i = 0; i < count; i++)
        {
            values[i] = source.Next();
        }

        return values;
    }

    // What one generated workload entry is: the decision Build takes once per slot,
    // named rather than left as a bare `Func<string>` whose reader can see an arity
    // and nothing about which workload the value belongs to. Each implementation
    // carries its own kind's size rule - a word's random length in [3, 8], a puzzle's
    // fixed 7 letters - and the Random the two kinds share.
    private interface IWorkloadEntrySource
    {
        // Produces the entry for the next slot, drawing from the workload's shared
        // Random, so the sequence of calls IS the workload: callers must not reorder
        // them or call one twice.
        string Next();
    }

    private sealed class WordSource(Random random) : IWorkloadEntrySource
    {
        public string Next()
        {
            var length = random.Next(MinWordLength, WordLengthUpperBound);
            return RandomLetters(random, length);
        }
    }

    private sealed class PuzzleSource(Random random) : IWorkloadEntrySource
    {
        public string Next() => RandomLetters(random, PuzzleLength);
    }
}
