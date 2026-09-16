using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationInStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// PermutationInStringSolution's, competing scans for the same yes/no question - rebuilding each
// window's counts against sliding the counts one position at a time - so a harness whose arms
// disagree is timing two different problems. Setup draws the haystack from one fixed seed, so the
// same Length must rebuild the same text.
//
// The agreement is weak by construction and says so rather than dressing it up: the fixture's
// alphabet excludes every letter of the harness's fixed pattern, so the answer is false at every
// window and the comparison can only catch an arm that ever answers true. The tests below pin that
// false as well, which is the honest reading of what this workload can prove - a workload that
// could exercise a real match is a harness decision, not a test's.
public sealed partial class PermutationInStringBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().HasPermutationByPerWindowRebuild(),
            BuildHarness().HasPermutationByPerWindowRebuild());

    [Fact]
    public void HasPermutationByPerWindowRebuild_AbsentPatternHaystack_AgreesWithHasPermutationBySlidingWindow()
    {
        var harness = BuildHarness();

        Assert.False(harness.HasPermutationBySlidingWindow());
        Assert.Equal(
            harness.HasPermutationBySlidingWindow(),
            harness.HasPermutationByPerWindowRebuild());
    }

    [Fact]
    public void HasPermutationBySlidingWindow_AbsentPatternHaystack_AgreesWithHasPermutationByPerWindowRebuild()
    {
        var harness = BuildHarness();

        Assert.False(harness.HasPermutationByPerWindowRebuild());
        Assert.Equal(
            harness.HasPermutationByPerWindowRebuild(),
            harness.HasPermutationBySlidingWindow());
    }

    private static PermutationInStringBenchmarks BuildHarness()
    {
        var harness = new PermutationInStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
