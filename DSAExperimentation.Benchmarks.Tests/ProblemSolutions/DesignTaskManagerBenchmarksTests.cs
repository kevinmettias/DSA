using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignTaskManagerBenchmarks (ARCHITECTURE 17.9): setup builds the initial
// tasks and a call script whose userIds all come from one bounded range, so the replay's sum of
// executed userIds has a band known from InitialTaskCount alone, without running either arm.
public sealed partial class DesignTaskManagerBenchmarksTests
{
    private const int SmallestInitialTaskCount = 200;

    // Every userId the workload ever holds is drawn below the initial task count, and the replay
    // sums the id each ExecTop reports - or zero when it found nothing to execute.
    private const long MinimumExecutedIdSum = 0;

    private const long MaximumExecutedIdSum =
        (long)SmallestInitialTaskCount * (SmallestInitialTaskCount - 1);

    // The script removes and edits only taskIds nothing has touched yet, then adds two fresh
    // tasks before every ExecTop - so the live task count only grows and each of the
    // InitialTaskCount execTop rounds has something to execute. Whatever the priorities drew,
    // every reported userId comes from the script's own id range, which is what the band pins.
    [Fact]
    public void LinearScan_EditAndGrowScript_SumsUserIdsFromTheScriptsOwnRange() =>
        Assert.InRange(BuildHarness().LinearScan(), MinimumExecutedIdSum, MaximumExecutedIdSum);

    private static DesignTaskManagerBenchmarks BuildHarness()
    {
        var harness = new DesignTaskManagerBenchmarks { InitialTaskCount = SmallestInitialTaskCount };
        harness.Setup();

        return harness;
    }
}
