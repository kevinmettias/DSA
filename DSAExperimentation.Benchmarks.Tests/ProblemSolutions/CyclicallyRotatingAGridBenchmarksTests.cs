using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CyclicallyRotatingAGridBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is the workload's documented shape. Setup
// fills a square template grid once and each strategy copies it before rotating; a rotation only moves
// cells within each ring, so the rotated grid holds exactly the template's values, which is what pins
// the workload's shape here rather than the private template itself.
public sealed partial class CyclicallyRotatingAGridBenchmarksTests
{
    private const int SmallestSize = 10;

    // The template counts its cells up from LC 1914's smallest cell value.
    private const int FirstCellValue = 1;

    [Fact]
    public void StepwiseQueue_TenByTenGridRotatedUnreducedSteps_HoldsExactlyTheTemplateValues()
    {
        var rotated = BuildHarness().StepwiseQueue();

        Assert.Equal(SmallestSize, rotated.Length);
        Assert.All(rotated, row => Assert.Equal(SmallestSize, row.Length));
        Assert.Equal(
            Enumerable.Range(FirstCellValue, SmallestSize * SmallestSize),
            rotated.SelectMany(row => row).Order());
    }

    private static CyclicallyRotatingAGridBenchmarks BuildHarness()
    {
        var harness = new CyclicallyRotatingAGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
