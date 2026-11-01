using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WaterAndJugProblemBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to agreeing with each other, which a shared wrong verdict would pass. Setup picks the target
// unreachable by construction (the class comment names jugX + jugY - 1 as never a multiple of the
// jugs' gcd), which is what makes the expected answer a decisive value rather than a re-reading of
// whichever arm answered first.
public sealed partial class WaterAndJugProblemBenchmarksTests
{
    private const int SmallestCapacity = 40;

    // The decisive value: an unreachable target is unmeasurable.
    [Fact]
    public void CanMeasureWaterByStackSearch_UnreachableTarget_ReportsImmeasurable() =>
        Assert.False(BuildHarness().CanMeasureWaterByStackSearch());

    private static WaterAndJugProblemBenchmarks BuildHarness()
    {
        var harness = new WaterAndJugProblemBenchmarks { Capacity = SmallestCapacity };
        harness.Setup();

        return harness;
    }
}
