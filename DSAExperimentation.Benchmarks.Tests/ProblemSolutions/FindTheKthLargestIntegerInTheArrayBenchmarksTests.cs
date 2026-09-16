using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheKthLargestIntegerInTheArrayBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - sorting every digit string against holding only
// the K largest in a size-K heap - so a harness whose arms disagree is timing two different
// problems. Setup draws mixed-length digit strings from one fixed seed, which is what makes the
// numeric order the solution imposes differ from string's own lexicographic one, so the same
// Length must rebuild the same array.
public sealed partial class FindTheKthLargestIntegerInTheArrayBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().FullSort(), BuildHarness().FullSort());

    [Fact]
    public void FullSort_SmallestLength_AgreesWithSizeKMinHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SizeKMinHeap(), harness.FullSort());
    }

    [Fact]
    public void SizeKMinHeap_SmallestLength_AgreesWithFullSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullSort(), harness.SizeKMinHeap());
    }

    private static FindTheKthLargestIntegerInTheArrayBenchmarks BuildHarness()
    {
        var harness = new FindTheKthLargestIntegerInTheArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
