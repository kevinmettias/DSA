using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMaximumAreaOfATriangleBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the textbook every-triple shoelace scan against this
// repo's own row/column span map - so a harness whose arms disagree has scored two different point
// sets. Both answers are twice the maximum area (or -1), one long, so they are compared directly.
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
    public void Setup_SamePointCount_RebuildsTheSameMaximumArea() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_CollidingRowsAndColumns_AgreesWithSpreadHashMap()
    {
        var harness = BuildHarness();

        Assert.True(harness.BruteForce() > 0);
        Assert.Equal(harness.SpreadHashMap(), harness.BruteForce());
    }

    [Fact]
    public void SpreadHashMap_CollidingRowsAndColumns_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.True(harness.SpreadHashMap() > 0);
        Assert.Equal(harness.BruteForce(), harness.SpreadHashMap());
    }

    private static FindMaximumAreaOfATriangleBenchmarks BuildHarness()
    {
        var harness = new FindMaximumAreaOfATriangleBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
