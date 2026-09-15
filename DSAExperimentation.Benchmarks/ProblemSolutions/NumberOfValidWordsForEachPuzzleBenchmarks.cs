using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfValidWordsForEachPuzzle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfValidWordsForEachPuzzleSolution's, the same
// methods NumberOfValidWordsForEachPuzzleTests proves correct. The words and
// puzzles are LeetCode's own input shape, so generating them is charged to
// [GlobalSetup] and each measured arm is handed them as-is.
[MemoryDiagnoser]
public class NumberOfValidWordsForEachPuzzleBenchmarks
{
    private const int PuzzleCount = 50;

    // LC problem number, reused as the deterministic workload seed.
    private const int WordSeed = 1178;

    private string[] _words = [];

    private string[] _puzzles = [];
    [Params(200, 4_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(WordSeed);
        _words = NumberOfValidWordsForEachPuzzleWorkloads.BuildWords(WordCount, random);
        _puzzles = NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, random);
    }

    [Benchmark(Baseline = true)]
    public List<int> MaskComparison() =>
        NumberOfValidWordsForEachPuzzleSolution.CountValidWordsByMaskComparison(_words, _puzzles);

    [Benchmark]
    public List<int> HashMapSubsetEnumeration() =>
        NumberOfValidWordsForEachPuzzleSolution.CountValidWordsByMaskSubsetEnumeration(_words, _puzzles);
}
