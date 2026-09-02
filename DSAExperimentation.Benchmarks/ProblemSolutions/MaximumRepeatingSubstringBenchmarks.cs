using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Repeating Substring (LC 1668): built-in string.Contains (baseline, the same
// "treat the BCL method as brute force" role it plays in RepeatedStringMatchBenchmarks)
// vs. this repo's own KMP-based PrefixFunctionSearch, both growing a candidate
// word+word+... one repeat at a time until it no longer occurs in sequence. sequence
// is built from Word repeated end to end so both strategies are forced through every
// increasing candidate length instead of failing on the very first repeat.
[MemoryDiagnoser]
public class MaximumRepeatingSubstringBenchmarks
{
    private const string Word = "ab";
    private const string EmptyCandidate = "";

    [Params(200, 5_000)]
    public int Length;

    private string _sequence = null!;

    [GlobalSetup]
    public void Setup()
    {
        var repeatedWords = Enumerable.Repeat(Word, Length / Word.Length);
        _sequence = string.Concat(repeatedWords);
    }

    [Benchmark(Baseline = true)]
    public int StringContains()
        => MaxRepeating(_sequence, Word, static (sequence, candidate) => sequence.Contains(candidate, StringComparison.Ordinal));

    [Benchmark]
    public int PrefixFunctionSearchContains()
        => MaxRepeating(_sequence, Word, static (sequence, candidate) => PrefixFunctionSearch.FindAll(sequence, candidate).Count > 0);

    private static int MaxRepeating(string sequence, string word, Func<string, string, bool> contains)
    {
        var repeats = 0;
        var candidate = EmptyCandidate;

        while (true)
        {
            var next = candidate + word;
            if (next.Length > sequence.Length || !contains(sequence, next))
            {
                return repeats;
            }

            candidate = next;
            repeats++;
        }
    }
}
