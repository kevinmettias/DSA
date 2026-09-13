using System.Text;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing and seeding for LC 1023. The matching itself is
// CamelcaseMatchingSolution's; what stays here is only how many queries to measure
// and how long a lowercase run to splice between the pattern's letters. Every
// generated query is a genuine match, so both arms are forced through their full
// scan instead of short-circuiting on an early uppercase mismatch.
internal static class CamelcaseQueryWorkloads
{
    // Each generated lowercase run is between 3 and 7 characters long.
    private const int MinRunLength = 3;
    private const int MaxRunLengthExclusive = 8;

    // Number of lowercase letters in the English alphabet.
    private const int AlphabetSize = 26;

    public static string[] BuildMatchingQueries(string pattern, int count, int seed)
    {
        var random = new Random(seed);

        return [.. Enumerable.Range(0, count).Select(_ => BuildMatchingQuery(pattern, random))];
    }

    // Intersperses a random-length lowercase run before, between and after each of
    // the pattern's characters, so the query always reduces back to the pattern.
    private static string BuildMatchingQuery(string pattern, Random random)
    {
        var builder = new StringBuilder();

        foreach (var c in pattern)
        {
            AppendLowercaseRun(builder, random);
            builder.Append(c);
        }

        AppendLowercaseRun(builder, random);
        return builder.ToString();
    }

    private static void AppendLowercaseRun(StringBuilder builder, Random random)
    {
        var runLength = random.Next(MinRunLength, MaxRunLengthExclusive);

        for (var i = 0; i < runLength; i++)
        {
            builder.Append((char)('a' + random.Next(AlphabetSize)));
        }
    }
}
