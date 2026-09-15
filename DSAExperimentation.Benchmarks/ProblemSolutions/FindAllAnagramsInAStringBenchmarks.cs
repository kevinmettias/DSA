using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindAllAnagramsInAString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindAllAnagramsInAStringSolution's, the same methods
// FindAllAnagramsInAStringTests proves correct. Rebuilding and comparing a fresh
// frequency map for every window start (O(n*m)) vs. a single sliding pass that
// maintains one window frequency map incrementally, using a running "matched
// distinct characters" counter instead of a full per-window comparison (O(n+m)).
[MemoryDiagnoser]
public class FindAllAnagramsInAStringBenchmarks
{
    private const string Pattern = "aeiou";
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 438;

    private string _s = "";

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        _s = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public List<int> PerWindowFrequencyRebuild() =>
        FindAllAnagramsInAStringSolution.FindAnagramIndicesByBruteForceRebuild(
            new ScannedText(_s), new AnagramPattern(Pattern));

    [Benchmark]
    public List<int> SlidingWindowFrequencyMap() =>
        FindAllAnagramsInAStringSolution.FindAnagramIndicesBySlidingWindow(
            new ScannedText(_s), new AnagramPattern(Pattern));
}
