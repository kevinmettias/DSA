using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LargestPerimeterTriangleBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - enumerating every triple against one sorted scan for
// the first triple whose two shorter sides clear the longest - so a harness whose arms disagree is
// timing two different problems. Setup draws the side lengths from one fixed seed, so the same
// Length must rebuild the same bag of sides; otherwise two published numbers were never comparable.
public sealed partial class LargestPerimeterTriangleBenchmarksTests
{
    private const int SmallestLength = 80;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameSides() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_RandomSides_AgreesWithSortThenScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortThenScan(), harness.BruteForce());
    }

    [Fact]
    public void SortThenScan_RandomSides_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SortThenScan());
    }

    private static LargestPerimeterTriangleBenchmarks BuildHarness()
    {
        var harness = new LargestPerimeterTriangleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
