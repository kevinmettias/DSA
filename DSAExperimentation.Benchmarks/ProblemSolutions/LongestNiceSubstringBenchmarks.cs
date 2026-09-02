using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Nice Substring (LC 1763): the naive "check every substring" baseline
// builds a fresh 128-entry presence table for each of the O(n^2) substrings and
// rescans it, O(n^3) overall. DivideAndConquer instead composes this repo's own
// Set<char> (LongestNiceSubstringTests precedent) to find the first character
// missing its opposite-case partner and recurse on the two halves - O(n^2)
// worst case, since no nice substring can ever cross that character. A tiny
// 4-letter mixed-case alphabet keeps both variants paying real work instead of
// exiting on an early mismatch.
[MemoryDiagnoser]
public class LongestNiceSubstringBenchmarks
{
    private const string Alphabet = "aAbB";
    private const int RandomSeed = 1763;
    private const int AsciiTableSize = 128;
    private const int MinimumNiceSubstringLength = 2;

    [Params(30, 150)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForceAllSubstrings()
    {
        var best = string.Empty;

        for (var start = 0; start < _text.Length; start++)
        {
            for (var end = start; end < _text.Length; end++)
            {
                var length = end - start + 1;

                if (length > best.Length && IsNice(start, end))
                {
                    best = _text.Substring(start, length);
                }
            }
        }

        return best;
    }

    private bool IsNice(int start, int end)
    {
        var present = new bool[AsciiTableSize];

        for (var i = start; i <= end; i++)
        {
            present[_text[i]] = true;
        }

        for (var i = start; i <= end; i++)
        {
            var c = _text[i];
            var partner = char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c);

            if (!present[partner])
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public string DivideAndConquer() => FindLongestNiceSubstring(_text);

    private static string FindLongestNiceSubstring(string s)
    {
        if (s.Length < MinimumNiceSubstringLength)
        {
            return string.Empty;
        }

        var present = BuildCharacterSet(s);

        for (var i = 0; i < s.Length; i++)
        {
            if (HasMissingPartner(present, s[i]))
            {
                return LongerHalf(s, i);
            }
        }

        return s;
    }

    private static Set<char> BuildCharacterSet(string s)
    {
        var present = new Set<char>();

        foreach (var c in s)
        {
            present.TryAdd(c);
        }

        return present;
    }

    private static string LongerHalf(string s, int splitIndex)
    {
        var left = FindLongestNiceSubstring(s[..splitIndex]);
        var right = FindLongestNiceSubstring(s[(splitIndex + 1)..]);
        return left.Length >= right.Length ? left : right;
    }

    private static bool HasMissingPartner(Set<char> present, char c)
        => char.IsUpper(c) ? !present.Has(char.ToLower(c)) : !present.Has(char.ToUpper(c));
}
