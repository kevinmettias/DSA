using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ConstrainedSubsequenceSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ConstrainedSubsequenceSumSolution's, the same methods
// ConstrainedSubsequenceSumTests proves correct. Values are random over a wide signed
// range so the window's running maximum keeps changing instead of settling on one
// dominant early value that would make the rescan arm look artificially cheap.
[MemoryDiagnoser]
public class ConstrainedSubsequenceSumBenchmarks
{
    private const int K = 50;

    // LC problem number, used as the deterministic benchmark input seed.
    private const int RandomSeed = 1425;

    // Symmetric bound for the random value range: values are drawn from
    // [-ValueRange, ValueRange) so the window's running maximum keeps changing.
    private const int ValueRange = 1_000;

    private int[] _nums = [];

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueRange, ValueRange)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanWindowEachPosition() => ConstrainedSubsequenceSumSolution.MaxSumByWindowRescan(_nums, K);

    [Benchmark]
    public int MonotonicDequeDp() => ConstrainedSubsequenceSumSolution.MaxSumByMonotonicDeque(_nums, K);
}
