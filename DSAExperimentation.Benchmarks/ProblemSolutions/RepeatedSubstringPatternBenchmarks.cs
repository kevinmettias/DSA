using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RepeatedSubstringPattern;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RepeatedSubstringPatternSolution's, the same methods
// RepeatedSubstringPatternTests proves correct. Text is random lowercase letters, so
// a genuine repeating period is astronomically unlikely and both strategies run to
// completion.
[MemoryDiagnoser]
public class RepeatedSubstringPatternBenchmarks
{
    private const int AlphabetSize = 26;

    private string _text = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _text = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public bool DivisorBruteForce() =>
        RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByDivisorBruteForce(_text);

    [Benchmark]
    public bool KmpFailureFunction() =>
        RepeatedSubstringPatternSolution.HasRepeatedSubstringPatternByKmpFailureFunction(_text);
}
