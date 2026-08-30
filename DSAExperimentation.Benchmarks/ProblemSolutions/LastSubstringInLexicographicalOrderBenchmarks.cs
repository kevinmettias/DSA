using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SuffixArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Last Substring in Lexicographical Order (LC 1163): pairwise O(n^2) suffix
// comparison (compare every candidate suffix against the current best via an
// ordinal span comparison) vs. this repo's own SuffixArray, which sorts every
// suffix in O(n log^2 n) and then reads the answer off as its final entry.
// A small 4-letter alphabet is used so many suffixes share long common prefixes,
// forcing both approaches through real character-by-character comparison work
// instead of resolving on the first character.
[MemoryDiagnoser]
public class LastSubstringInLexicographicalOrderBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _text = string.Create(Length, random, static (span, rng) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = (char)('a' + rng.Next(0, 4));
            }
        });
    }

    [Benchmark(Baseline = true)]
    public string PairwiseComparison()
    {
        var bestStart = 0;

        for (var candidate = 1; candidate < _text.Length; candidate++)
        {
            var candidateSpan = _text.AsSpan(candidate);
            var bestSpan = _text.AsSpan(bestStart);

            if (candidateSpan.CompareTo(bestSpan, StringComparison.Ordinal) > 0)
            {
                bestStart = candidate;
            }
        }

        return _text[bestStart..];
    }

    [Benchmark]
    public string SuffixArrayLookup()
    {
        var suffixArray = new SuffixArray(_text);
        return _text[suffixArray.Suffixes[^1]..];
    }
}
