using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationInStringBenchmarks (ARCHITECTURE 17.9): the baseline's verdict
// against the one the workload's shape fixes, which the generic check cannot supply - it holds the
// sliding window to the baseline. The fixture's alphabet excludes every letter of the harness's fixed
// pattern, so the answer is false at every window; a workload that could exercise a real match is a
// harness decision, not a test's.
public sealed partial class PermutationInStringBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void HasPermutationByPerWindowRebuild_AbsentPatternHaystack_ReportsNoPermutation() =>
        Assert.False(BuildHarness().HasPermutationByPerWindowRebuild());

    private static PermutationInStringBenchmarks BuildHarness()
    {
        var harness = new PermutationInStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
