using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaxChunksToMakeSorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxChunksToMakeSortedSolution's, the same methods
// MaxChunksToMakeSortedTests proves correct, run over a random permutation of
// 0..Length-1 so neither strategy gets to special-case an already-sorted input.
[MemoryDiagnoser]
public class MaxChunksToMakeSortedBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, Length).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _values = values;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaxChunksToMakeSortedSolution.MaxChunksByBruteForce(_values);

    [Benchmark]
    public int RunningMaxScan() => MaxChunksToMakeSortedSolution.MaxChunksByRunningMaxScan(_values);
}
