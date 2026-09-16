using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IPOBenchmarks (ARCHITECTURE 17.9): both arms are competing strategies
// for the same question - the O(k*n) linear rescan per round against the two-heap greedy - so a
// harness whose arms disagree is timing two different problems. Both arms answer with the one
// maximized capital total, an int, compared directly. The projects are seeded, so the same
// ProjectCount must rebuild the same profits and capitals and therefore the same total.
public sealed partial class IPOBenchmarksTests
{
    private const int SmallestProjectCount = 200;

    [Fact]
    public void Setup_SameProjectCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScanPerRound(), BuildHarness().LinearScanPerRound());

    [Fact]
    public void LinearScanPerRound_MaximizedCapital_AgreesWithTwoHeapGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoHeapGreedy(), harness.LinearScanPerRound());
    }

    [Fact]
    public void TwoHeapGreedy_MaximizedCapital_AgreesWithLinearScanPerRound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanPerRound(), harness.TwoHeapGreedy());
    }

    private static IPOBenchmarks BuildHarness()
    {
        var harness = new IPOBenchmarks { ProjectCount = SmallestProjectCount };
        harness.Setup();

        return harness;
    }
}
