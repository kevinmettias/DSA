using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Palindromic Substring (LC 5): the textbook O(n^2) expand-around-every-
// center baseline vs. this repo's own O(n) Manacher primitive. Text is drawn from a
// tiny 4-letter alphabet rather than a full character range, so repeated runs are
// common and ExpandAroundCenter actually pays its quadratic worst case instead of
// exiting most expansions after one comparison.
[MemoryDiagnoser]
public class LongestPalindromicSubstringBenchmarks
{
    [Params(500, 8_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(23);
        const string Alphabet = "abcd";
        _text = new string(Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int ExpandAroundCenter()
    {
        var bestLength = 0;

        for (var center = 0; center < _text.Length; center++)
        {
            bestLength = Math.Max(bestLength, ExpandFrom(center, center));
            bestLength = Math.Max(bestLength, ExpandFrom(center, center + 1));
        }

        return bestLength;
    }

    private int ExpandFrom(int left, int right)
    {
        while (left >= 0 && right < _text.Length && _text[left] == _text[right])
        {
            left--;
            right++;
        }

        return right - left - 1;
    }

    [Benchmark]
    public int Manacher()
    {
        var (_, length) = DSAExperimentation.Algorithms.StringMatching.Manacher.FindLongestPalindromicSubstring(_text);
        return length;
    }
}
