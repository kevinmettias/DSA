using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GenerateRandomPointInACircleBenchmarks (ARCHITECTURE 17.9). Both arms are
// GenerateRandomPointInACircleSolution's Design-style generators, and each builds its own unseeded
// System.Random inside the call, so the two arms can never be compared value for value - an
// equality assertion here would be asserting that two independent random streams agree, which is
// not a property either arm has. What both arms genuinely share is the distribution they exist to
// sample: points uniform by area inside the circle centred on (XCenter, YCenter), so the mean of
// the X coordinates they return has to sit at XCenter. Each arm is asserted against that shared
// oracle under a named relative tolerance wide enough to absorb the sampling noise of the smallest
// Draws setting (the standard error of the mean of Draws uniform-in-circle X draws is
// Radius / (2 * sqrt(Draws)), about 0.16 at a thousand draws, so this tolerance is several times
// that) and narrow enough that a generator centred anywhere else fails it. This is weaker than arm
// agreement and is reported as such: it witnesses that both arms sample the circle they were built
// for, not that they produced the same points.
public sealed partial class GenerateRandomPointInACircleBenchmarksTests
{
    private const int SmallestDraws = 1_000;

    // Radius and centre are the values the harness constructs both generators with.
    private const double Radius = 10.0;
    private const double XCenter = 5.0;

    // Sampling noise, not numeric slack: see the tolerance note above.
    private const double RelativeTolerance = 0.15;

    [Fact]
    public void RejectionSampling_SmallestDrawBatch_ReturnsPointsCentredOnTheConfiguredCenter() =>
        Assert.InRange(
            MeanX(BuildHarness().RejectionSampling()),
            XCenter - (RelativeTolerance * Radius),
            XCenter + (RelativeTolerance * Radius));

    [Fact]
    public void ClosedFormPolar_SmallestDrawBatch_ReturnsPointsCentredOnTheConfiguredCenter() =>
        Assert.InRange(
            MeanX(BuildHarness().ClosedFormPolar()),
            XCenter - (RelativeTolerance * Radius),
            XCenter + (RelativeTolerance * Radius));

    private static double MeanX(double sumOfX) => sumOfX / SmallestDraws;

    private static GenerateRandomPointInACircleBenchmarks BuildHarness() => new() { Draws = SmallestDraws };
}
