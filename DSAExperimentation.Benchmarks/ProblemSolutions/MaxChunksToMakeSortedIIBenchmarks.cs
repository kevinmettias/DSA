using DSAExperimentation.LeetCode.MaxChunksToMakeSortedII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxChunksToMakeSortedIISolution's, the same methods
// MaxChunksToMakeSortedIISolutionTests proves correct. Values are a random permutation so
// no candidate boundary is confirmed or ruled out on the very next element. Length
// stops at LC 768's 2,000-element cap.
public class MaxChunksToMakeSortedIIBenchmarks
{
    private const int RandomSeed = 768; private int[] _values = [];

    // LC problem number

    [Params(200, 2_000)]
    public int Length { get; set; }

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
