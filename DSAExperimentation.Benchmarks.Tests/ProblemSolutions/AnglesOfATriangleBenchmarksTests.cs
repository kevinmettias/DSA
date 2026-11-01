using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AnglesOfATriangleBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each
// other, and this asserts the workload is a triangle at all - LC 3899 answers an empty array for side lengths
// that fail the strict triangle inequality, and two arms agreeing on that empty answer would time nothing. The
// class has no [Params] at all: a triangle is always three sides, so there is no size axis to tune and the
// harness is a bare initializer plus Setup, which draws its valid side triple from the fixture's one seed.
public sealed partial class AnglesOfATriangleBenchmarksTests
{
    // The three internal angles LC 3899's answer is defined to hold, one per side.
    private const int TriangleAngleCount = 3;

    [Fact]
    public void LawOfCosines_FixedValidTriangle_ReturnsThreeAngles() =>
        Assert.Equal(TriangleAngleCount, BuildHarness().LawOfCosines().Length);

    private static AnglesOfATriangleBenchmarks BuildHarness()
    {
        var harness = new AnglesOfATriangleBenchmarks();
        harness.Setup();

        return harness;
    }
}
