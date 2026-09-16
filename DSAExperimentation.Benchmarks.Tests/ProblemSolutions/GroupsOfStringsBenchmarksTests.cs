using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GroupsOfStringsBenchmarks (ARCHITECTURE 17.9): both arms are
// GroupsOfStringsSolution's - the O(n^2) pairwise popcount scan against the HashMap turned into
// O(1) neighbour lookups - so a harness whose arms disagree is timing two different problems. Both
// arms answer with the same two readings in the same fixed order, the group count followed by the
// largest group's size, so AnswerText.Of compares that pair element by element. [GlobalSetup]
// builds each word as a random distinct-letter subset of the alphabet - the problem's own
// precondition - so real add/delete/replace connections occur rather than every word standing
// alone, and the mask pass is charged to setup rather than to the grouping being measured. The
// shuffle runs off one seed, so the same WordCount must rebuild the same words and the same masks.
public sealed partial class GroupsOfStringsBenchmarksTests
{
    private const int SmallestWordCount = 200;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PairwisePopCountScan()),
            AnswerText.Of(BuildHarness().PairwisePopCountScan()));

    [Fact]
    public void PairwisePopCountScan_SeededLetterSetMasks_AgreesWithHashMapNeighborLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.HashMapNeighborLookup()), AnswerText.Of(harness.PairwisePopCountScan()));
    }

    [Fact]
    public void HashMapNeighborLookup_SeededLetterSetMasks_AgreesWithPairwisePopCountScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.PairwisePopCountScan()), AnswerText.Of(harness.HashMapNeighborLookup()));
    }

    private static GroupsOfStringsBenchmarks BuildHarness()
    {
        var harness = new GroupsOfStringsBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
