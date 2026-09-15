using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.WordSearch;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WordSearchSolution's, the same methods
// WordSearchTests proves correct.
[MemoryDiagnoser]
public class WordSearchBenchmarks
{
    private const string TargetWord = "ABCCED";

    private char[][] _board = [];

    [GlobalSetup]
    public void Setup() => _board = [['A', 'B', 'C', 'E'], ['S', 'F', 'C', 'S'], ['A', 'D', 'E', 'E']];

    [Benchmark(Baseline = true)]
    public bool BruteForceDfs() => WordSearchSolution.ExistByBruteForceDfs(_board, TargetWord);

    [Benchmark]
    public bool Backtrack() => WordSearchSolution.ExistByBacktrack(_board, TargetWord);
}
