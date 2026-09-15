using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HIndex;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HIndexSolution's, the same methods HIndexTests
// proves correct.
[MemoryDiagnoser]
public class HIndexBenchmarks
{
    private const int RandomSeed = 274; // LC problem number
    private const int CitationCountExclusiveBound = 1_000;

    private int[] _citations = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _citations = Enumerable.Range(0, Length).Select(_ => random.Next(0, CitationCountExclusiveBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => HIndexSolution.HIndexByBruteForce(_citations);

    [Benchmark]
    public int MergeSortScan() => HIndexSolution.HIndexByMergeSortScan(_citations);
}
