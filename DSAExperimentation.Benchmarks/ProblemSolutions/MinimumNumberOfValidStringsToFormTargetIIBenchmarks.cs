using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfValidStringsToFormTargetII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfValidStringsToFormTargetIISolution's,
// the same methods MinimumNumberOfValidStringsToFormTargetIITests proves correct.
// TargetLength stays a fraction of this problem's own 5*10^4 bound so BruteForce's
// O(target.Length * sum(words[i].Length)) arm still completes - ZFunctionAcrossWords
// is the strategy the real bound actually needs. A small 4-letter alphabet keeps
// words and target overlapping heavily so both arms do real comparison work.
[MemoryDiagnoser]
public class MinimumNumberOfValidStringsToFormTargetIIBenchmarks
{
    private const int RandomSeed = 3292; // LeetCode problem number
    private const int WordCount = 20;
    private const int WordLength = 200;
    private const string Alphabet = "abcd";

    [Params(2_000, 8_000)]
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
    public int BruteForce() => MinimumNumberOfValidStringsToFormTargetIISolution.MinValidStringsByBruteForce(_words, _target);

    [Benchmark]
    public int ZFunctionAcrossWords() => MinimumNumberOfValidStringsToFormTargetIISolution.MinValidStringsByZFunctionAcrossWords(_words, _target);
}
