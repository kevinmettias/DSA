using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RussianDollEnvelopesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a naive sort followed by a quadratic dynamic program against
// the sort-then-patience-sorting O(n log n) sweep - so a harness whose arms disagree is timing two
// different problems. Setup draws the (width, height) pairs from one fixed seed, so the same Length
// must rebuild the same pairs; otherwise two published numbers were never comparable in the first
// place.
//
// Both arms read the hoisted pairs without writing to them, so one harness is safe to call twice in
// either order and the single-harness rule holds. The chain length depends on how many of the random
// pairs nest, which the fixture's documented shape does not decide; the arms are reconciled against
// each other.
public sealed partial class RussianDollEnvelopesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveSortAndDp(), BuildHarness().NaiveSortAndDp());

    [Fact]
    public void NaiveSortAndDp_SeededEnvelopes_AgreesWithSortThenPatienceSorting()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortThenPatienceSorting(), harness.NaiveSortAndDp());
    }

    [Fact]
    public void SortThenPatienceSorting_SeededEnvelopes_AgreesWithNaiveSortAndDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveSortAndDp(), harness.SortThenPatienceSorting());
    }

    private static RussianDollEnvelopesBenchmarks BuildHarness()
    {
        var harness = new RussianDollEnvelopesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
