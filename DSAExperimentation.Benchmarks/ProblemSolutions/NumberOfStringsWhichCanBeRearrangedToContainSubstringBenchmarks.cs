using DSAExperimentation.LeetCode.Solutions.NumberOfStringsWhichCanBeRearrangedToContainSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution's, the same
// methods ...Tests proves correct. The state DP is O(n) with a small constant
// factor per step; the inclusion-exclusion closed form is O(log n) via modular
// exponentiation (ModularPower) regardless of the string length,
// so the gap should widen as StringLength grows toward LC 2930's real 1e5 bound.
public class NumberOfStringsWhichCanBeRearrangedToContainSubstringBenchmarks
{
    [Params(1_000, 100_000)]
    public int StringLength { get; set; }

    [Benchmark(Baseline = true)]
    public int StateDp() =>
        NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution.CountRearrangeableStringsByStateDp(StringLength);

    [Benchmark]
    public int InclusionExclusion() =>
        NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution
            .CountRearrangeableStringsByInclusionExclusion(StringLength);
}
