using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaxChunksToMakeSortedII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxChunksToMakeSortedIISolution's, the same methods
// MaxChunksToMakeSortedIITests proves correct. Values are a random permutation so
// no candidate boundary is confirmed or ruled out on the very next element.
[MemoryDiagnoser]
public class MaxChunksToMakeSortedIIBenchmarks
{
    private const int RandomSeed = 768; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSuffixRescan() => MaxChunksToMakeSortedIISolution.MaxChunksByBruteForceSuffixRescan(_values);

    [Benchmark]
    public int MonotonicStackMerge() => MaxChunksToMakeSortedIISolution.MaxChunksByMonotonicStackMerge(_values);
}
