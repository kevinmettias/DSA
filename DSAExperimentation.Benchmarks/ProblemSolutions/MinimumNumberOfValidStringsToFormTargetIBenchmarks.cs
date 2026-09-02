using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfValidStringsToFormTargetI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfValidStringsToFormTargetISolution's,
// the same methods MinimumNumberOfValidStringsToFormTargetITests proves correct.
// A small 4-letter alphabet keeps words and target overlapping heavily, so
// BruteForce's nested comparison actually does the character-by-character work
// its complexity implies rather than bailing out on the first character.
[MemoryDiagnoser]
public class MinimumNumberOfValidStringsToFormTargetIBenchmarks
{
    private const int RandomSeed = 3291; // LeetCode problem number
    private const int WordCount = 20;
    private const int WordLength = 50;
    private const string Alphabet = "abcd";

    [Params(500, 2_000)]
    public int TargetLength;

    private string[] _words = null!;
    private string _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount).Select(_ => RandomString(random, WordLength)).ToArray();
        _target = RandomString(random, TargetLength);
    }

    private static string RandomString(Random random, int length)
        => string.Create(length, random, static (span, rng) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = Alphabet[rng.Next(Alphabet.Length)];
            }
        });

    [Benchmark(Baseline = true)]
    public int BruteForce() => MinimumNumberOfValidStringsToFormTargetISolution.MinValidStringsByBruteForce(_words, _target);

    [Benchmark]
    public int ZFunctionAcrossWords() => MinimumNumberOfValidStringsToFormTargetISolution.MinValidStringsByZFunctionAcrossWords(_words, _target);
}
