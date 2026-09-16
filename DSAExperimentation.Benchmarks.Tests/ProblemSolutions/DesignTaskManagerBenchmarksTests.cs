using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignTaskManagerBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a full linear scan for the highest-priority task against this
// repo's own lazy-deletion heap - so a harness whose arms disagree is timing two different
// problems. Setup builds the initial tasks and the whole call script from one fixed seed, so the
// same InitialTaskCount must rebuild the same workload.
public sealed partial class DesignTaskManagerBenchmarksTests
{
    private const int SmallestInitialTaskCount = 200;

    // Every userId the workload ever holds is drawn below the initial task count, and the replay
    // sums the id each ExecTop reports - or zero when it found nothing to execute.
    private const long MinimumExecutedIdSum = 0;

    private const long MaximumExecutedIdSum =
        (long)SmallestInitialTaskCount * (SmallestInitialTaskCount - 1);

    [Fact]
    public void Setup_SameInitialTaskCount_RebuildsTheSameTaskAndScriptWorkload()
    {
        // The script removes and edits only taskIds nothing has touched yet, then adds two fresh
        // tasks before every ExecTop - so the live task count only grows and each of the
        // InitialTaskCount execTop rounds has something to execute. Whatever the priorities drew,
        // every reported userId comes from the script's own id range, which is what the band pins.
        Assert.InRange(BuildHarness().LinearScan(), MinimumExecutedIdSum, MaximumExecutedIdSum);
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_EditAndGrowScript_AgreesWithLazyDeletionHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyDeletionHeap(), harness.LinearScan());
    }

    [Fact]
    public void LazyDeletionHeap_EditAndGrowScript_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.LazyDeletionHeap());
    }

    private static DesignTaskManagerBenchmarks BuildHarness()
    {
        var harness = new DesignTaskManagerBenchmarks { InitialTaskCount = SmallestInitialTaskCount };
        harness.Setup();

        return harness;
    }
}
