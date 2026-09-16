using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumHeightTreesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - recomputing the height from every candidate root against peeling
// leaves inwards - so a harness whose arms disagree is timing two different trees. Both arms answer with
// the same set of minimum-height roots, which for an even node count is a single centre node; the
// returning order is this answer's order, since both strategies report the roots in node index order,
// so AnswerText.Of and not OfUnorderedSet is the rendering that keeps each root scored against its own
// position. Setup builds one random recursive tree from a fixed seed, so the same NodeCount must
// rebuild the same adjacency.
public sealed partial class MinimumHeightTreesBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTree() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().HeightFromEveryNode()),
            AnswerText.Of(BuildHarness().HeightFromEveryNode()));

    [Fact]
    public void HeightFromEveryNode_RandomRecursiveTree_AgreesWithLeafPeeling()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.LeafPeeling()), AnswerText.Of(harness.HeightFromEveryNode()));
    }

    [Fact]
    public void LeafPeeling_RandomRecursiveTree_AgreesWithHeightFromEveryNode()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.HeightFromEveryNode()), AnswerText.Of(harness.LeafPeeling()));
    }

    private static MinimumHeightTreesBenchmarks BuildHarness()
    {
        var harness = new MinimumHeightTreesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
