using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GCDSortOfAnArrayBenchmarks (ARCHITECTURE 17.9): both arms are
// GCDSortOfAnArraySolution's - the pairwise-gcd sweep over a hand-rolled parent array against the
// value-to-prime-factor union over this repo's own DisjointSet - so a harness whose arms disagree is
// timing two different problems. Values are drawn as products of one small shared prime pool, so
// real shared-factor chains occur and the verdict is not trivially decided by any single value.
// Both arms answer with a bare bool, so agreement says the two strategies reached the same verdict
// on the same values, and nothing stronger: the fixture's own products are what make that verdict
// the same one either way. Setup draws the values once off that seed, so the same Length must
// rebuild the same array.
public sealed partial class GCDSortOfAnArrayBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanBeSortedByPairwiseGcdUnionFind(),
            BuildHarness().CanBeSortedByPairwiseGcdUnionFind());

    [Fact]
    public void CanBeSortedByPairwiseGcdUnionFind_SharedPrimePool_AgreesWithPrimeFactorDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanBeSortedByPrimeFactorDisjointSet(), harness.CanBeSortedByPairwiseGcdUnionFind());
    }

    [Fact]
    public void CanBeSortedByPrimeFactorDisjointSet_SharedPrimePool_AgreesWithPairwiseGcdUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanBeSortedByPairwiseGcdUnionFind(), harness.CanBeSortedByPrimeFactorDisjointSet());
    }

    private static GCDSortOfAnArrayBenchmarks BuildHarness()
    {
        var harness = new GCDSortOfAnArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
