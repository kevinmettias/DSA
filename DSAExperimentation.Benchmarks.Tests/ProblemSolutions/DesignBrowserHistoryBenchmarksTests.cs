using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignBrowserHistoryBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a List<string> with a RemoveRange truncation
// against this repo's own DynamicArray<string> popping the discarded tail one element at a time -
// so a harness whose arms disagree is timing two different problems. Setup builds both url runs
// from OperationCount, so the same OperationCount must rebuild the same visit script, and that
// script's shape is what decides the url each arm reports at the end.
public sealed partial class DesignBrowserHistoryBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    // Mirrors the benchmark's own home page: the replay's last Back call is the value each arm
    // reports, so the test has to state which page that has to land on.
    private const string HomePageUrl = "home.com";

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameVisitScript()
    {
        // Every iteration visits one url, steps two pages back, then visits a branch url - so each
        // iteration ends with the history truncated to [home, branch] and the cursor back on
        // home. The last Back of the script therefore reports the home page, which Visit's
        // truncation is what makes true: a Visit that appended instead would leave a longer
        // history behind and report some other page instead.
        Assert.Equal(HomePageUrl, BuildHarness().ListBacked());
        Assert.Equal(BuildHarness().ListBacked(), BuildHarness().ListBacked());
    }

    [Fact]
    public void ListBacked_VisitBackBranchCycle_AgreesWithDynamicArrayBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicArrayBacked(), harness.ListBacked());
    }

    [Fact]
    public void DynamicArrayBacked_VisitBackBranchCycle_AgreesWithListBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListBacked(), harness.DynamicArrayBacked());
    }

    private static DesignBrowserHistoryBenchmarks BuildHarness()
    {
        var harness = new DesignBrowserHistoryBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
