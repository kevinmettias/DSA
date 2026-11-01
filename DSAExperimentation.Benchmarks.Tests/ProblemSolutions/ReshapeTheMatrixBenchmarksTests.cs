using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReshapeTheMatrixBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the shape the workload is built to produce. Setup fixes the
// target shape at half as many columns as the source and twice as many rows, so the total cell count
// is preserved and the reshaping branch is taken rather than the "return the original unchanged"
// branch - which the baseline's row and column counts show without reference to either arm.
public sealed partial class ReshapeTheMatrixBenchmarksTests
{
    private const int SmallestRows = 20;

    // The benchmark reshapes into twice as many rows and half as many columns of the same width.
    private const int TargetReshapeFactor = 2;
    private const int SourceColumnCount = 8;
    private const int ExpectedTargetRowCount = SmallestRows * TargetReshapeFactor;
    private const int ExpectedTargetColumnCount = SourceColumnCount / TargetReshapeFactor;

    [Fact]
    public void LinearIndexDivMod_DoubledRows_ReturnsTheTargetShape()
    {
        var reshaped = BuildHarness().LinearIndexDivMod();

        Assert.Equal(ExpectedTargetRowCount, reshaped.Length);
        Assert.Equal(ExpectedTargetColumnCount, reshaped[0].Length);
    }

    private static ReshapeTheMatrixBenchmarks BuildHarness()
    {
        var harness = new ReshapeTheMatrixBenchmarks { Rows = SmallestRows };
        harness.Setup();

        return harness;
    }
}
