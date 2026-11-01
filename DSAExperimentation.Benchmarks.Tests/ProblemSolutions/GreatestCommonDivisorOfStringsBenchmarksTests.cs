using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GreatestCommonDivisorOfStringsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on the smallest workload, so this asserts
// only the answer's length, which the fixture's own arithmetic fixes independently of either arm:
// both inputs are the same unit repeated a different, coprime number of times, so the two lengths
// are coprime multiples of the unit length, and the longest string dividing both is the unit -
// whatever unit [GlobalSetup] happens to draw.
public sealed partial class GreatestCommonDivisorOfStringsBenchmarksTests
{
    private const int SmallestUnitLength = 20;

    [Fact]
    public void ConcatenationEqualityCheck_CoprimeRepeatCounts_ReturnsAStringOneUnitLong() =>
        Assert.Equal(SmallestUnitLength, BuildHarness().ConcatenationEqualityCheck().Length);

    private static GreatestCommonDivisorOfStringsBenchmarks BuildHarness()
    {
        var harness = new GreatestCommonDivisorOfStringsBenchmarks { UnitLength = SmallestUnitLength };
        harness.Setup();

        return harness;
    }
}
