using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.VowelsGameInAString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are VowelsGameInAStringSolution's, the same methods
// VowelsGameInAStringTests proves correct. Length stays small - CanAliceWinByGameSearch
// explores actual game states, so unlike a typical O(n^2) baseline its cost
// does not grow smoothly with n, and a mixed vowel/consonant string can already
// reach many thousands of recursive states by length 30-40.
[MemoryDiagnoser]
public class VowelsGameInAStringBenchmarks
{
    private const int RandomSeed = 3227;
    private const string Alphabet = "aeioubcdfghjklmnpqrst";

    private string _text = "";

    [Params(8, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByGameSearch() => VowelsGameInAStringSolution.CanAliceWinByGameSearch(_text);

    [Benchmark]
    public bool CanAliceWinByVowelExistence() => VowelsGameInAStringSolution.CanAliceWinByVowelExistence(_text);
}
