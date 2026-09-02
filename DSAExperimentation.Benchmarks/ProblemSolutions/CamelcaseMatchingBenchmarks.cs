using BenchmarkDotNet.Attributes;
using System.Text;
using System.Text.RegularExpressions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Camelcase Matching (LC 1023): the common editorial shortcut - splice "[a-z]*"
// between every pattern character and let the regex engine's own backtracking
// state machine do the subsequence-with-uppercase-lock matching - vs. a direct
// two-pointer scan per query, the same "no repo data structure needed" shape
// CamelcaseMatchingTests itself uses and the same per-item-scan baseline shape
// NumberOfMatchingSubsequencesBenchmarks' TwoPointerPerWord already uses. Both
// are linear in query length, but the regex engine's per-character overhead
// makes it measurably slower than the plain index walk.
[MemoryDiagnoser]
public class CamelcaseMatchingBenchmarks
{
    private const string Pattern = "FB";

    // LC problem number, used as the deterministic seed for query generation.
    private const int RandomSeed = 1023;

    // Any run of lowercase letters, spliced between/around the pattern's uppercase letters.
    private const string LowercaseRun = "[a-z]*";

    [Params(500, 10_000)]
    public int QueryCount;

    private string[] _queries = null!;
    private Regex _regex = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _queries = Enumerable.Range(0, QueryCount).Select(_ => GenerateMatchingQuery(random)).ToArray();
        _regex = new Regex(LowercaseRun + string.Join(LowercaseRun, Pattern.Select(c => c.ToString())) + LowercaseRun);
    }

    [Benchmark(Baseline = true)]
    public int RegexPerQuery()
    {
        var matches = 0;

        foreach (var query in _queries)
        {
            if (_regex.IsMatch(query))
            {
                matches++;
            }
        }

        return matches;
    }

    [Benchmark]
    public int TwoPointerPerQuery()
    {
        var matches = 0;

        foreach (var query in _queries)
        {
            if (Matches(query, Pattern))
            {
                matches++;
            }
        }

        return matches;
    }

    private static bool Matches(string query, string pattern)
    {
        var p = 0;

        foreach (var c in query)
        {
            if (p < pattern.Length && pattern[p] == c)
            {
                p++;
                continue;
            }

            if (char.IsUpper(c))
            {
                return false;
            }
        }

        return p == pattern.Length;
    }

    // Builds a query that always reduces to Pattern ("FB") by interspersing a
    // random-length lowercase run before, between, and after each uppercase
    // letter - every generated query is a genuine match, forcing both
    // approaches through their full-length scan instead of an early
    // mismatch short-circuit.
    private static string GenerateMatchingQuery(Random random)
    {
        var builder = new StringBuilder();

        foreach (var c in Pattern)
        {
            AppendLowercaseRun(builder, random);
            builder.Append(c);
        }

        AppendLowercaseRun(builder, random);
        return builder.ToString();
    }

    // Each generated lowercase run is between 3 and 7 characters long.
    private const int MinLowercaseRunLength = 3;
    private const int MaxLowercaseRunLengthExclusive = 8;

    // Number of lowercase letters in the English alphabet.
    private const int LowercaseAlphabetSize = 26;

    private static void AppendLowercaseRun(StringBuilder builder, Random random)
    {
        var runLength = random.Next(MinLowercaseRunLength, MaxLowercaseRunLengthExclusive);

        for (var i = 0; i < runLength; i++)
        {
            builder.Append((char)('a' + random.Next(LowercaseAlphabetSize)));
        }
    }
}
