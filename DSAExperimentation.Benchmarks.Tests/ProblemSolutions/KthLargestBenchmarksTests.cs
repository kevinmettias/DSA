using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthLargestBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the kth largest value in a seeded array - so a harness whose
// arms disagree is timing two different problems. Both arms answer with the bare value, and the
// full sort orders every element while the size-k min-heap orders only k of them, so the two
// orders have to be reconciled down to the one position the problem actually asks about.
public sealed partial class KthLargestBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(
            BuildHarness().FullSort(),
            BuildHarness().FullSort());

    [Fact]
    public void FullSort_SeededValues_AgreesWithSizeKMinHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SizeKMinHeap(), harness.FullSort());
    }

    [Fact]
    public void SizeKMinHeap_SeededValues_AgreesWithFullSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullSort(), harness.SizeKMinHeap());
    }

    private static KthLargestBenchmarks BuildHarness()
    {
        var harness = new KthLargestBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
