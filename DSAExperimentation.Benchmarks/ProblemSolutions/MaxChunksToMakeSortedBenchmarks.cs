using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaxChunksToMakeSorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxChunksToMakeSortedSolution's, the same methods
// MaxChunksToMakeSortedSolutionTests proves correct, run over a random permutation of
// 0..Length-1 so neither strategy gets to special-case an already-sorted input. Length
// stops at LC 769's 10-element cap.
public class MaxChunksToMakeSortedBenchmarks
{
    private int[] _values = [];

    [Params(3, 10)]
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
