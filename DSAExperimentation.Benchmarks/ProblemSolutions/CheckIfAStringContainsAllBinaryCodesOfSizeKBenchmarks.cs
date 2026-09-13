using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheckIfAStringContainsAllBinaryCodesOfSizeK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfAStringContainsAllBinaryCodesOfSizeKSolution's,
// the same methods CheckIfAStringContainsAllBinaryCodesOfSizeKTests proves correct.
// The text is built once in [GlobalSetup] and deliberately contains every length-K
// code, so the per-code substring search runs its full 2^K searches rather than
// exiting early on a missing one.
[MemoryDiagnoser]
public class CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks
{
    [Params(8, 10)]
    public int K;

    private string _text = null!;

    [GlobalSetup]
    public void Setup() => _text = BinaryCodeTextWorkloads.BuildCoveringText(K);

    [Benchmark(Baseline = true)]
    public bool BruteForceSubstringSearch() =>
        CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesByCodeSubstringSearch(_text, K);

    [Benchmark]
    public bool SlidingBitmaskWithSet() =>
        CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesBySlidingBitmask(_text, K);
}
