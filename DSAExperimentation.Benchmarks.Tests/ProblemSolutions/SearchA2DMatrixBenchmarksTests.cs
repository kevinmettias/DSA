using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchA2DMatrixBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the verdict the workload is built to have. Setup numbers every
// cell 0..Size*Size-1 in row order without any randomness, so the benchmark's target - that last
// value - is present by construction; the test asserts that presence, because a reintroduced
// per-row counter would silently make the searched value absent and turn both arms into the -1
// fast path.
public sealed partial class SearchA2DMatrixBenchmarksTests
{
    private const int SmallestSize = 40;

    [Fact]
    public void HasTargetByLinearScan_RowOrderGrid_FindsTheLargestValue() =>
        Assert.True(BuildHarness().HasTargetByLinearScan());

    private static SearchA2DMatrixBenchmarks BuildHarness()
    {
        var harness = new SearchA2DMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
