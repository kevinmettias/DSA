using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountElementsWithAtLeastKGreaterValuesBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - comparing every element against every other
// against one upper-bound bisection per element over the sorted copy - so a harness whose arms
// disagree is timing two different problems. Both arms return an int, so they are compared directly.
// Setup draws the values from one fixed seed and sorts them, so the same Length must rebuild the same
// pair of arrays: the sorted copy is what the hoisted arm reads and is charged to Setup rather than
// to either measured loop.
public sealed partial class CountElementsWithAtLeastKGreaterValuesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameSortedCopy() =>
        Assert.Equal(BuildHarness().SortedUpperBound(), BuildHarness().SortedUpperBound());

    [Fact]
    public void BruteForce_TwoHundredValues_AgreesWithSortedUpperBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedUpperBound(), harness.BruteForce());
    }

    [Fact]
    public void SortedUpperBound_TwoHundredValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SortedUpperBound());
    }

    private static CountElementsWithAtLeastKGreaterValuesBenchmarks BuildHarness()
    {
        var harness = new CountElementsWithAtLeastKGreaterValuesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
