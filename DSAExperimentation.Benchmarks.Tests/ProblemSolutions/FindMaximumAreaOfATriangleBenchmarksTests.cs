using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMaximumAreaOfATriangleBenchmarks (ARCHITECTURE 17.9): the generic arms
// check only compares the arms with each other, and arms that all answer -1 would agree too, so this
// holds the baseline to what Setup's workload guarantees. The answer is twice the maximum area, or
// -1 when no triangle exists.
//
// Setup draws its points from a grid narrower than the point count, so rows and columns collide and
// axis-parallel triangles genuinely exist: the answer is a positive doubled area rather than -1,
// which is what makes "not -1" a real assertion about the workload instead of a restatement of the
// arms' own output.
public sealed partial class FindMaximumAreaOfATriangleBenchmarksTests
{
    // The smaller of Setup's [Params(60, 300)] point counts.
    private const int SmallestPointCount = 60;

    [Fact]
    public void BruteForce_CollidingRowsAndColumns_FindsATriangle() =>
        Assert.True(BuildHarness().BruteForce() > 0);

    private static FindMaximumAreaOfATriangleBenchmarks BuildHarness()
    {
        var harness = new FindMaximumAreaOfATriangleBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
