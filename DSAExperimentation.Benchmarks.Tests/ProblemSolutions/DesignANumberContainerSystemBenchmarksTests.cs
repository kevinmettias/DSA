using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignANumberContainerSystemBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests
// cannot pin: a bound on every reported index that follows from the workload's construction rather than from either
// arm. Setup builds the call script from one fixed seed: each index first gets its own distinct number, then a fifth
// of the indices are reassigned. Each arm returns the index every find reported, in order, and each is either LC
// 2349's -1 for a number nothing holds or an index below Count.
public sealed partial class DesignANumberContainerSystemBenchmarksTests
{
    private const int SmallestCount = 200;

    // LC 2349's answer when no index holds the number.
    private const int NotFound = -1;

    [Fact]
    public void LinearScan_TwoHundredSeededIndices_ReportsOnlyIndicesTheScriptAssigned() =>
        AssertReportsOnlyAssignedIndices(BuildHarness().LinearScan());

    [Fact]
    public void LazyDeletionHeap_TwoHundredSeededIndices_ReportsOnlyIndicesTheScriptAssigned() =>
        AssertReportsOnlyAssignedIndices(BuildHarness().LazyDeletionHeap());

    private static void AssertReportsOnlyAssignedIndices(int[] found) =>
        Assert.All(found, index => Assert.InRange(index, NotFound, SmallestCount - 1));

    private static DesignANumberContainerSystemBenchmarks BuildHarness()
    {
        var harness = new DesignANumberContainerSystemBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
