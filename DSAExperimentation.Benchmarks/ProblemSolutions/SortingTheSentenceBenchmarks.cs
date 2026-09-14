using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SortingTheSentence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortingTheSentenceSolution's, the same methods
// SortingTheSentenceTests proves correct. The workload is generalized beyond the
// problem's real 1-9-word/single-digit constraint (the same "scale past the strict
// LeetCode bound to exercise real complexity" convention AddTwoNumbersBenchmarks
// and RelativeSortArrayBenchmarks already use) so a numeric position suffix of any
// length replaces the single trailing digit. Each arm is handed the prepared word
// array its hoisted overload takes, so the shuffle is charged to [GlobalSetup]
// rather than to the sort being measured.
[MemoryDiagnoser]
public class SortingTheSentenceBenchmarks
{
    private const int RandomSeed = 1859; // LC problem number

    [Params(200, 2_000)]
    public int Length;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var positions = Enumerable.Range(0, Length).OrderBy(_ => random.Next()).ToArray();
        _words = positions.Select(position => $"word{position}").ToArray();
    }

    [Benchmark(Baseline = true)]
    public string PositionScan() => SortingTheSentenceSolution.SortSentenceByPositionScan(_words);

    [Benchmark]
    public string MergeSortByPosition() => SortingTheSentenceSolution.SortSentenceByMergeSort(_words);
}
