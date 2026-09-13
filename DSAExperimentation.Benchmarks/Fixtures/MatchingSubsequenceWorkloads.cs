namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing and seeding for LC 792. s is drawn from a 25-letter
// alphabet and every word ends in the excluded 26th letter, so no word ever
// matches: the per-word two-pointer arm is forced through its full
// O(words * s.Length) worst case instead of short-circuiting on an early match,
// which is exactly the case the waiting-bucket pass wins.
internal static class MatchingSubsequenceWorkloads
{
    public const int TextLength = 20_000;

    private const char UnreachableChar = 'z';

    private const int WordLength = 4;

    private const int AlphabetSize = 25;

    public static string BuildText(Random random, int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(chars);
    }

    public static string[] BuildUnmatchableWords(Random random, int count) =>
        [.. Enumerable.Range(0, count).Select(_ => BuildText(random, WordLength) + UnreachableChar)];
}
