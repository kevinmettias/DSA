using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfScoresOfBuiltStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfScoresOfBuiltStringsSolution's, the same
// methods SumOfScoresOfBuiltStringsTests proves correct - the O(n^2) suffix
// comparison against this repo's own ZFunction.Compute, O(n).
//
// A two-letter alphabet is used deliberately: it maximizes self-overlap, which is
// the brute force's actual worst case. A high-entropy string lets it bail out of
// nearly every comparison after one character and the two arms look far closer
// than they are.
[MemoryDiagnoser]
public class SumOfScoresOfBuiltStringsBenchmarks
{
    private const int RandomSeed = 2223; // LC problem number
    private const int AlphabetSize = 2;

    private string _text = "";

    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string([.. Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize)))]);
    }

    [Benchmark(Baseline = true)]
    public long BruteForceSuffixComparison() =>
        SumOfScoresOfBuiltStringsSolution.SumScoresBySuffixComparison(_text);

    [Benchmark]
    public long ZFunctionOverText() =>
        SumOfScoresOfBuiltStringsSolution.SumScoresByZFunction(_text);
}
