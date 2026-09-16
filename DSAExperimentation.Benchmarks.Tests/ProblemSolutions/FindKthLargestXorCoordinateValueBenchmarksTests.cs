using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindKthLargestXorCoordinateValueBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a full sort of every coordinate value against a
// size-k min-heap - over the same shared prefix-XOR pass, so a harness whose arms disagree has
// selected the kth largest out of two different value multisets. Both answers are one int, so they
// are compared directly.
//
// Setup's matrix is the same seeded random grid for a given Side, so the same parameters must
// rebuild the same coordinate values and therefore the same kth largest.
public sealed partial class FindKthLargestXorCoordinateValueBenchmarksTests
{
    // The smaller of Setup's [Params(50, 300)] side lengths.
    private const int SmallestSide = 50;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameKthLargestValue() =>
        Assert.Equal(BuildHarness().FullSort(), BuildHarness().FullSort());

    [Fact]
    public void FullSort_RandomSeededMatrix_AgreesWithSizeKMinHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SizeKMinHeap(), harness.FullSort());
    }

    [Fact]
    public void SizeKMinHeap_RandomSeededMatrix_AgreesWithFullSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullSort(), harness.SizeKMinHeap());
    }

    private static FindKthLargestXorCoordinateValueBenchmarks BuildHarness()
    {
        var harness = new FindKthLargestXorCoordinateValueBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
