using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PropertiesGraphBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - how many connected components the property graph has - so a
// harness whose arms disagree is timing two different problems. Both return the component count
// as a scalar, so the arms are compared directly. Setup builds the property matrix from one fixed
// seed, so the same RowCount must rebuild the same matrix; otherwise two published numbers were
// never comparable in the first place.
public sealed partial class PropertiesGraphBenchmarksTests
{
    private const int SmallestRowCount = 20;

    [Fact]
    public void Setup_SameRowCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededPropertyMatrix_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.DisjointSet());
    }

    [Fact]
    public void DisjointSet_SeededPropertyMatrix_AgreesWithTheBruteForceArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSet(), harness.BruteForce());
    }

    private static PropertiesGraphBenchmarks BuildHarness()
    {
        var harness = new PropertiesGraphBenchmarks { RowCount = SmallestRowCount };
        harness.Setup();

        return harness;
    }
}
