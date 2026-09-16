using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaxChunksToMakeSorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxChunksToMakeSortedSolution's, the same methods
// MaxChunksToMakeSortedTests proves correct, run over a random permutation of
// 0..Length-1 so neither strategy gets to special-case an already-sorted input.
[MemoryDiagnoser]
public class MaxChunksToMakeSortedBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = SeededSequences.ShuffledZeroTo(Length, seed: 1);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaxChunksToMakeSortedSolution.MaxChunksByBruteForce(_values);

    [Benchmark]
    public int RunningMaxScan() => MaxChunksToMakeSortedSolution.MaxChunksByRunningMaxScan(_values);
}
