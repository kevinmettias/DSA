using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestPalindromicSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestPalindromicSubstringSolution's, the same
// methods LongestPalindromicSubstringTests proves correct. Text is drawn from a
// tiny 4-letter alphabet rather than a full character range, so repeated runs are
// common and ExpandAroundCenter actually pays its quadratic worst case instead of
// exiting most expansions after one comparison.
[MemoryDiagnoser]
public class LongestPalindromicSubstringBenchmarks
{
    private const int RandomSeed = 23;
    private const string Alphabet = "abcd";

    private string _text = "";

    [Params(500, 8_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string ExpandAroundCenter() => LongestPalindromicSubstringSolution.FindLongestPalindromeByExpandAroundCenter(_text);

    [Benchmark]
    public string Manacher() => LongestPalindromicSubstringSolution.FindLongestPalindromeByManacher(_text);
}
