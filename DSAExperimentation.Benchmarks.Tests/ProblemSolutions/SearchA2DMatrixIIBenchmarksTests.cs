using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchA2DMatrixIIBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the verdict the workload is built to have. Setup numbers every
// cell 0..Size*Size-1 in row order and the arms search for the class's own below-everything target,
// so the test asserts the negative the fixture is built to force - which is also what keeps all
// three arms on their full worst-case walk instead of an early exit.
public sealed partial class SearchA2DMatrixIIBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void HasTargetByFullScan_BelowRangeTarget_ReportsAbsent() =>
        Assert.False(BuildHarness().HasTargetByFullScan());

    private static SearchA2DMatrixIIBenchmarks BuildHarness()
    {
        var harness = new SearchA2DMatrixIIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
