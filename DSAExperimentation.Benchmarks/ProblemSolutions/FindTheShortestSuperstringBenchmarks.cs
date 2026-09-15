using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheShortestSuperstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheShortestSuperstringSolution's, the same methods
// FindTheShortestSuperstringTests proves correct - the textbook permutation brute
// force (O(n! * n)) against the bitmask-TSP DP built on this repo's own Memoizer
// (O(2^n * n^2)). The overlap matrix is the prepared input both hoisted overloads
// take, so it is built once in [GlobalSetup] and neither arm is charged for the
// string-overlap preprocessing they share. Word count stays small ([6, 9]) because
// n! overtakes 2^n * n^2 fast enough that the brute force would otherwise dominate
// the run.
[MemoryDiagnoser]
public class FindTheShortestSuperstringBenchmarks
{
    // LC problem number, reused as the deterministic word seed.
    private const int WordSeed = 943;
    private const int WordLength = 5;

    private WordOverlaps _overlaps = null!;

    [Params(6, 9)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var words = SuperstringWordWorkloads.BuildWords(WordCount, WordLength, WordSeed);

        _overlaps = WordOverlaps.Build(words);
    }

    [Benchmark(Baseline = true)]
    public string BruteForcePermutations() =>
        FindTheShortestSuperstringSolution.ShortestSuperstringByPermutations(_overlaps);

    [Benchmark]
    public string MemoizedBitmaskDp() =>
        FindTheShortestSuperstringSolution.ShortestSuperstringByMemoizedBitmask(_overlaps);
}
