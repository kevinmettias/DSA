using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeTheNumberOfPartitionsAfterOperations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeTheNumberOfPartitionsAfterOperationsSolution's,
// the same methods MaximizeTheNumberOfPartitionsAfterOperationsTests proves correct
// (TwoSumBenchmarks precedent). BruteForceRecolor re-walks the whole string for
// every one of ~25 * Length candidate recolorings; BitmaskMemo instead shares work
// across candidates via Memoizer's cache, so Length is kept modest enough for the
// O(Length^2) baseline to still finish in reasonable benchmark time.
[MemoryDiagnoser]
public class MaximizeTheNumberOfPartitionsAfterOperationsBenchmarks
{
    private const int Alphabet = 6; // small alphabet forces frequent forced cuts, exercising both arms' cut logic
    private const int DistinctLimit = 3;
    private const int Seed = 3003;

    private string _text = "";

    [Params(100, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(Alphabet));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecolor() =>
        MaximizeTheNumberOfPartitionsAfterOperationsSolution.MaxPartitionsByBruteForceRecolor(_text, DistinctLimit);

    [Benchmark]
    public int BitmaskMemo() =>
        MaximizeTheNumberOfPartitionsAfterOperationsSolution.MaxPartitionsByBitmaskMemo(_text, DistinctLimit);
}
