using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TheNumberOfBeautifulSubsetsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - walking all 2^n bitmasks and checking every pair
// afterwards against the pruned search that never makes an illegal inclusion - so a harness whose
// arms disagree is timing two different problems. Both arms return the subset count as an int, so
// they are compared directly. Setup draws the values from a fixed seed, so the same Length must
// rebuild the same workload.
public sealed partial class TheNumberOfBeautifulSubsetsBenchmarksTests
{
    private const int SmallestLength = 12;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().GenerateThenFilter(), BuildHarness().GenerateThenFilter());

    [Fact]
    public void GenerateThenFilter_SmallestLength_AgreesWithPrunedBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrunedBacktracking(), harness.GenerateThenFilter());
    }

    [Fact]
    public void PrunedBacktracking_SmallestLength_AgreesWithGenerateThenFilter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GenerateThenFilter(), harness.PrunedBacktracking());
    }

    private static TheNumberOfBeautifulSubsetsBenchmarks BuildHarness()
    {
        var harness = new TheNumberOfBeautifulSubsetsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
