using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AnglesOfATriangleBenchmarks (ARCHITECTURE 17.9): its two arms are
// AnglesOfATriangleSolution's competing strategies for the same question - a third Acos call against deriving the
// third angle from the triangle's angle sum - so a harness whose arms disagree is measuring two different
// triangles. The class has no [Params] at all: a triangle is always three sides, so there is no size axis to tune
// and the harness is a bare initializer plus Setup, which draws its valid side triple from the fixture's one seed.
// Angles are doubles, so every comparison here carries the class's own named tolerance rather than testing exact
// double equality.
public sealed partial class AnglesOfATriangleBenchmarksTests
{
    // The three internal angles LC 3899's answer is defined to hold, one per side.
    private const int TriangleAngleCount = 3;

    // The two arms reach the third angle by different routes, so agreement is expected only to
    // the precision of the Acos and subtraction they each perform.
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void Setup_FixedValidTriangle_RebuildsTheSameAngles() =>
        Assert.True(AgreeWithinTolerance(BuildHarness().LawOfCosines(), BuildHarness().LawOfCosines()));

    [Fact]
    public void LawOfCosines_FixedValidTriangle_AgreesWithAngleSum()
    {
        var harness = BuildHarness();

        Assert.Equal(TriangleAngleCount, harness.LawOfCosines().Length);
        Assert.True(AgreeWithinTolerance(harness.AngleSum(), harness.LawOfCosines()));
    }

    [Fact]
    public void AngleSum_FixedValidTriangle_AgreesWithLawOfCosines()
    {
        var harness = BuildHarness();

        Assert.Equal(TriangleAngleCount, harness.AngleSum().Length);
        Assert.True(AgreeWithinTolerance(harness.LawOfCosines(), harness.AngleSum()));
    }

    private static AnglesOfATriangleBenchmarks BuildHarness()
    {
        var harness = new AnglesOfATriangleBenchmarks();
        harness.Setup();

        return harness;
    }

    private static bool AgreeWithinTolerance(double[] first, double[] second) =>
        first.Length == second.Length
        && first.Zip(second).All(pair => Math.Abs(pair.First - pair.Second) <= RelativeTolerance);
}
