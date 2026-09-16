using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AirplaneSeatAssignmentProbabilityBenchmarks (ARCHITECTURE 17.9): its two arms are
// AirplaneSeatAssignmentProbabilitySolution's - the O(n^2) memoized recurrence and the closed form it provably
// reduces to - so a harness whose arms disagree is timing two different problems. Both arms are [Params]-driven
// with no [GlobalSetup], so the harness is constructed per plane size and the arms called directly. The answer
// is a probability, so every comparison here carries the class's own relative tolerance rather than testing
// exact double equality.
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
    public void MemoizedRecursion_SmallPlaneSize_AgreesWithClosedForm()
    {
        var harness = Harness(SmallestPlaneSize);

        Assert.Equal(harness.ClosedForm(), harness.MemoizedRecursion(), RelativeTolerance);
    }

    [Fact]
    public void MemoizedRecursion_LargePlaneSize_AgreesWithClosedForm()
    {
        var harness = Harness(LargestPlaneSize);

        Assert.Equal(harness.ClosedForm(), harness.MemoizedRecursion(), RelativeTolerance);
    }

    // The recurrence summed over every shorter seat count is the definition the closed form has to match, so
    // its own answer is asserted against the constant too, not only against the other arm.
    [Fact]
    public void MemoizedRecursion_SmallPlaneSize_IsOneHalf() =>
        Assert.Equal(
            ExpectedNonFirstSeatProbability,
            Harness(SmallestPlaneSize).MemoizedRecursion(),
            RelativeTolerance);

    private static AirplaneSeatAssignmentProbabilityBenchmarks Harness(int planeSize) =>
        new() { PlaneSize = planeSize };
}
