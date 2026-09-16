using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheckIfAStringContainsAllBinaryCodesOfSizeK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfAStringContainsAllBinaryCodesOfSizeKSolution's,
// the same methods CheckIfAStringContainsAllBinaryCodesOfSizeKTests proves correct.
// The text is built once in [GlobalSetup] and deliberately contains every code of
// the requested length, so the per-code substring search runs every one of its 2^k
// searches rather than exiting early on a missing one.
[MemoryDiagnoser]
public class CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks
{
    private string _text = "";

    [Params(8, 10)]
    public int CodeLength { get; set; }

    [GlobalSetup]
    public void Setup() => _text = BinaryCodeTextWorkloads.BuildCoveringText(CodeLength);

    [Benchmark(Baseline = true)]
    public bool HasAllCodesByCodeSubstringSearch() =>
        CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesByCodeSubstringSearch(_text, CodeLength);

    [Benchmark]
    public bool HasAllCodesBySlidingBitmask() =>
        CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesBySlidingBitmask(_text, CodeLength);
}
