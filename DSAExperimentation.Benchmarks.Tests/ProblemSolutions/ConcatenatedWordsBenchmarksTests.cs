using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConcatenatedWordsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a Set-backed DP scan against the pruned Trie walk - so a
// harness whose arms disagree is timing two different problems. Setup builds the word list and both
// lookup structures it feeds them from, so the same Length must rebuild the same tiled candidate,
// the same Set and the same Trie; otherwise two published numbers were never comparable.
//
// AnswerText.Of, not OfUnorderedSet: the arms return a flat list of whole words, not a collection of
// groups, so there is no inner order for a set rendering to protect and no unfixed outer order to
// paper over. The workload's documented shape - one long tiling of the dictionary word plus that
// word itself - means exactly one entry is a concatenation, and it is the tile.
public sealed partial class ConcatenatedWordsBenchmarksTests
{
    private const int SmallestLength = 600;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWordsAndLookups()
    {
        Assert.Single(BuildHarness().HashSetUnboundedScan());
        Assert.Equal(
            AnswerText.Of(BuildHarness().HashSetUnboundedScan()),
            AnswerText.Of(BuildHarness().HashSetUnboundedScan()));
    }

    [Fact]
    public void HashSetUnboundedScan_TiledDictionaryWord_AgreesWithTriePrunedMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.TriePrunedMemoized()), AnswerText.Of(harness.HashSetUnboundedScan()));
    }

    [Fact]
    public void TriePrunedMemoized_TiledDictionaryWord_AgreesWithHashSetUnboundedScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.HashSetUnboundedScan()), AnswerText.Of(harness.TriePrunedMemoized()));
    }

    private static ConcatenatedWordsBenchmarks BuildHarness()
    {
        var harness = new ConcatenatedWordsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
