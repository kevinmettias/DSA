namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 395 - a random string drawn from a small
// 4-letter alphabet rather than the full 26, so that with K's threshold most
// characters clear it and the divide-and-conquer strategy actually gets to
// recurse instead of splitting on the first character it sees.
internal static class LongestSubstringWithAtLeastKRepeatingCharactersWorkloads
{
    private const int AlphabetSize = 4;

    public static string BuildString(int length, int seed)
    {
        var random = new Random(seed);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(chars);
    }
}
