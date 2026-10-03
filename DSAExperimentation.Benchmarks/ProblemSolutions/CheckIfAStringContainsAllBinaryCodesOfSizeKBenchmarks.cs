using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheckIfAStringContainsAllBinaryCodesOfSizeK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfAStringContainsAllBinaryCodesOfSizeKSolution's,
// the same methods CheckIfAStringContainsAllBinaryCodesOfSizeKSolutionTests proves correct.
// The text is built once per code length in [GlobalSetup] and deliberately contains
// every code of that length, so the per-code substring search runs every one of its
// 2^k searches rather than exiting early on a missing one.
//
// Sizes are per arm. The substring search scans the whole 2^k * k text once per code,
// so it stops at a code length of 10; the sliding bitmask's single pass runs on to 15,
// the longest code whose covering text (491,520 characters) fits LC 1461's bound of
// 5 * 10^5, and the two are compared at the lengths both run.
public class CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks
{
    private Dictionary<int, string> _textByCodeLength = [];

    public static IEnumerable<int> SubstringSearchSizes => [8, 10];

    public static IEnumerable<int> SlidingBitmaskSizes => [.. SubstringSearchSizes, 12, 15];

    // Every code length any arm runs is built here, outside the timed region; an arm looks
    // its own up.
    [GlobalSetup]
    public void Setup() =>
        _textByCodeLength = SlidingBitmaskSizes.ToDictionary(
            codeLength => codeLength,
            BinaryCodeTextWorkloads.BuildCoveringText);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(SubstringSearchSizes))]
    public bool HasAllCodesByCodeSubstringSearch(int codeLength) =>
        CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesByCodeSubstringSearch(
            _textByCodeLength[codeLength], codeLength);

    [Benchmark]
    [ArgumentsSource(nameof(SlidingBitmaskSizes))]
    public bool HasAllCodesBySlidingBitmask(int codeLength) =>
        CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesBySlidingBitmask(
            _textByCodeLength[codeLength], codeLength);
}
