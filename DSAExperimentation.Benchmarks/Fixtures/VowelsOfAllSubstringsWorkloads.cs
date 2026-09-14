namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2063 - everything about the strategies
// themselves now lives in LeetCode.VowelsOfAllSubstrings; what stays here is only
// how long a word to build and how to seed it. Letters are drawn uniformly from
// the whole lowercase alphabet, so roughly 5 in 26 positions are vowels and
// neither strategy meets a degenerate all-vowel or vowel-free word.
internal static class VowelsOfAllSubstringsWorkloads
{
    private const int LowercaseAlphabetSize = 26;

    public static string BuildRandomLowercaseWord(int length, int seed)
    {
        var random = new Random(seed);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, LowercaseAlphabetSize));
        }

        return new string(chars);
    }
}
