using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Palindromic Substrings (LC 647): the textbook O(n^2) expand-around-every-center
// baseline (counting every successful expansion instead of just tracking the
// longest one, the counting variant of LongestPalindromicSubstringBenchmarks'
// ExpandAroundCenter) vs. this repo's own O(n) Manacher primitive - summing every
// radius counts every palindromic substring in one pass instead of finding just the
// longest one. Text is drawn from a tiny 4-letter alphabet rather than a full
// character range, so repeated runs are common and ExpandAroundCenter actually pays
// its quadratic worst case instead of exiting most expansions after one comparison.
[MemoryDiagnoser]
public class PalindromicSubstringsBenchmarks
{
    [Params(500, 8_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(29);
        const string Alphabet = "abcd";
        _text = new string(Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int ExpandAroundCenter()
    {
        var count = 0;

        for (var center = 0; center < _text.Length; center++)
        {
            count += CountExpansionsFrom(center, center);
            count += CountExpansionsFrom(center, center + 1);
        }

        return count;
    }

    private int CountExpansionsFrom(int left, int right)
    {
        var count = 0;

        while (left >= 0 && right < _text.Length && _text[left] == _text[right])
        {
            count++;
            left--;
            right++;
        }

        return count;
    }

    [Benchmark]
    public int Manacher()
    {
        var count = 0;

        foreach (var radius in DSAExperimentation.Algorithms.StringMatching.Manacher.ComputeOddRadii(_text))
        {
            count += radius;
        }

        foreach (var radius in DSAExperimentation.Algorithms.StringMatching.Manacher.ComputeEvenRadii(_text))
        {
            count += radius;
        }

        return count;
    }
}
