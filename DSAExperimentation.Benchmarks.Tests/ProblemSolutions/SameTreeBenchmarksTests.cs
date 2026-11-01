using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SameTreeBenchmarks (ARCHITECTURE 17.9). Arm agreement is BenchmarkArmsTests'
// job; this pins the verdict the workload is built to have. [GlobalSetup] builds both trees
// separately from the same level-order array, so the two are structurally equal with equal values at
// every node - a pair the answer is decisively true for, which is asserted here rather than read back
// out of either arm.
public sealed partial class SameTreeBenchmarksTests
{
    private const int SmallestNodeCount = 10;

    [Fact]
    public void IsSameByRecursiveCompare_IdenticallyBuiltTrees_ReportsThemTheSame() =>
        Assert.True(BuildHarness().IsSameByRecursiveCompare());

    private static SameTreeBenchmarks BuildHarness()
    {
        var harness = new SameTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
