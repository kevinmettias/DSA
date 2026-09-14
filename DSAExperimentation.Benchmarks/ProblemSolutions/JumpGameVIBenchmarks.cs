using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.JumpGameVI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are JumpGameVISolution's, the same methods JumpGameVITests
// proves correct. Values are random over a wide signed range so the window's running
// maximum keeps changing instead of settling on one dominant early value that would
// make the rescan arm look artificially cheap - the same workload shape
// ConstrainedSubsequenceSumBenchmarks uses for LC 1425's own trailing window.
[MemoryDiagnoser]
public class JumpGameVIBenchmarks
{
    private const int K = 50;

    // LC problem number, used as the deterministic benchmark input seed.
    private const int RandomSeed = 1696;

    // Symmetric bound for the random value range: values are drawn from
    // [-ValueRange, ValueRange) so the window's running maximum keeps changing.
    private const int ValueRange = 1_000;

    [Params(2_000, 20_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueRange, ValueRange)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanWindowEachPosition() => JumpGameVISolution.MaxResultByWindowRescan(_nums, K);

    [Benchmark]
    public int MonotonicDequeDp() => JumpGameVISolution.MaxResultByMonotonicDeque(_nums, K);
}
