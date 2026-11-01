using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AirplaneSeatAssignmentProbabilityBenchmarks (ARCHITECTURE 17.9): the generic check
// compares the arms only at the smallest PlaneSize, so this pins the closed form to LC 1227's constant at both
// [Params] values and holds the O(n^2) memoized recurrence to it at the largest, a size that check never
// reaches. Both arms are [Params]-driven with no [GlobalSetup], so the harness is constructed per plane size and
// the arms called directly. The answer is a probability, so every comparison here carries the class's own
// relative tolerance rather than testing exact double equality.
public sealed partial class AirplaneSeatAssignmentProbabilityBenchmarksTests
{
    // LC 1227's answer for every plane past the first: the first and last seats are symmetric.
    private const double ExpectedNonFirstSeatProbability = 0.5;

    private const double RelativeTolerance = 1e-9;

    private const int SmallestPlaneSize = 100;
    private const int LargestPlaneSize = 1_000;

    [Fact]
    public void ClosedForm_PlaneSizesPastTheFirst_ReturnsOneHalf()
    {
        Assert.Equal(
            ExpectedNonFirstSeatProbability,
            Harness(SmallestPlaneSize).ClosedForm(),
            RelativeTolerance);
        Assert.Equal(
            ExpectedNonFirstSeatProbability,
            Harness(LargestPlaneSize).ClosedForm(),
            RelativeTolerance);
    }

    [Fact]
    public void MemoizedRecursion_LargePlaneSize_AgreesWithClosedForm()
    {
        var harness = Harness(LargestPlaneSize);

        Assert.Equal(harness.ClosedForm(), harness.MemoizedRecursion(), RelativeTolerance);
    }

    private static AirplaneSeatAssignmentProbabilityBenchmarks Harness(int planeSize) =>
        new() { PlaneSize = planeSize };
}
