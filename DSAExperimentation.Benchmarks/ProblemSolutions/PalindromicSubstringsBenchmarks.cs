using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PalindromicSubstrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromicSubstringsSolution's, the same methods
// PalindromicSubstringsTests proves correct. Text is drawn from a tiny 4-letter
// alphabet rather than a full character range, so repeated runs are common and
// ExpandAroundCenter actually pays its quadratic worst case instead of exiting most
// expansions after one comparison.
[MemoryDiagnoser]
public class PalindromicSubstringsBenchmarks
{
    private const int RandomSeed = 29;
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
    public int ExpandAroundCenter() => PalindromicSubstringsSolution.CountSubstringsByExpandAroundCenter(_text);

    [Benchmark]
    public int Manacher() => PalindromicSubstringsSolution.CountSubstringsByManacher(_text);
}
