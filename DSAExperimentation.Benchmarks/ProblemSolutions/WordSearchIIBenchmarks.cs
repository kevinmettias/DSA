using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.WordSearchII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WordSearchIISolution's, the same methods
// WordSearchIITests proves correct. Words are random (mostly absent from the
// board) so neither strategy short-circuits, forcing both through their real
// worst-case cost.
[MemoryDiagnoser]
public class WordSearchIIBenchmarks
{
    private const int BoardSize = 8;
    private const int WordLength = 4;
    private const int RandomSeed = 17;
    private const int AlphabetSize = 26;

    [Params(20, 200)]
    public int WordCount;

    private char[][] _board = null!;
    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _board = Enumerable.Range(0, BoardSize)
            .Select(_ => Enumerable.Range(0, BoardSize).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray())
            .ToArray();
        _words = Enumerable.Range(0, WordCount)
            .Select(_ => new string(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray()))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PerWordBruteForce() => WordSearchIISolution.FindWordsByBruteForceDfs(_board, _words).Count;

    [Benchmark]
    public int TriePrunedSearch() => WordSearchIISolution.FindWordsByTrieBacktrack(_board, _words).Count;
}
