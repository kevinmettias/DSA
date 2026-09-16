using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ReconstructTreeWorkloads (ARCHITECTURE 17.7). The reading depends on LC 1719's
// `pairs` being the FULL ancestor/descendant relation of a forest - a root with several disjoint chains
// hanging off it - so every non-root node clears the initial degree check and both strategies are forced
// through the whole candidate-parent/subset-check walk rather than short-circuiting early.
public sealed partial class ReconstructTreeWorkloadsTests
{
    private const int NodeCount = 50;
    private const int ChainCount = 5;
    private const int Root = 0;
    private const int FirstChainNode = 1;
    private const int PairFieldCount = 2;

    // Five chains of (NodeCount - 1) / ChainCount = 9 nodes each: a node pairs with the root and with
    // every node above it on its own chain, so one chain contributes 1 + 2 + ... + 9 = 45 pairs.
    private const int ChainLength = (NodeCount - FirstChainNode) / ChainCount;
    private const int ExpectedPairCount =
        ChainCount * (ChainLength * (ChainLength + FirstChainNode) / AlgorithmConstants.HalvingFactor);

    [Fact]
    public void BuildStarOfChainsPairs_EveryPair_IsATwoNodeRowKeepingTheAncestorBeforeTheDescendant()
    {
        var pairs = ReconstructTreeWorkloads.BuildStarOfChainsPairs(NodeCount, ChainCount);

        Assert.All(pairs, pair => Assert.Equal(PairFieldCount, pair.Length));
        Assert.All(pairs, pair => Assert.InRange(pair[0], Root, NodeCount - 1));
        Assert.All(pairs, pair => Assert.InRange(pair[1], FirstChainNode, NodeCount - 1));
        Assert.All(pairs, pair => Assert.True(pair[0] < pair[1]));
    }

    [Fact]
    public void BuildStarOfChainsPairs_PairCount_MatchesTheFullAncestorRelationOfEveryChainOnItsOwn() =>
        Assert.Equal(
            ExpectedPairCount,
            ReconstructTreeWorkloads.BuildStarOfChainsPairs(NodeCount, ChainCount).Length);

    // The fixture-derived oracle: the pairs are the full ancestor relation of the documented forest, not
    // a sample of it, and the generator's own comment says so. Derived here from the shape the comment
    // describes - equal chains hanging off the root - rather than by re-reading the generator's pointers.
    [Fact]
    public void BuildStarOfChainsPairs_Pairs_AreTheFullAncestorRelationOfTheDocumentedForest() =>
        Assert.Equal(
            AnswerText.Of(ExpectedAncestorPairs()),
            AnswerText.Of(ReconstructTreeWorkloads.BuildStarOfChainsPairs(NodeCount, ChainCount)));

    // No pair may cross two chains: LC 1719's relation is within a chain plus the root, so a pair whose
    // ancestor is neither the root nor on the descendant's own chain is a relation the problem never
    // states, and one the strategies would have to reject as a candidate parent.
    [Fact]
    public void BuildStarOfChainsPairs_NoPair_LeavesTheDescendantsOwnChainExceptThroughTheRoot() =>
        Assert.All(
            ReconstructTreeWorkloads.BuildStarOfChainsPairs(NodeCount, ChainCount),
            pair => Assert.True(pair[0] == Root || ChainOf(pair[0]) == ChainOf(pair[1])));

    // Every node on a chain descends from the root, so the root has to appear as the ancestor of each of
    // them - the relation the strategies read to seat the root of the tree.
    [Fact]
    public void BuildStarOfChainsPairs_Root_IsAnAncestorOfEveryNodeOnAChain()
    {
        var pairs = ReconstructTreeWorkloads.BuildStarOfChainsPairs(NodeCount, ChainCount);

        foreach (var node in Enumerable.Range(FirstChainNode, ChainCount * ChainLength))
        {
            Assert.Contains(pairs, pair => pair[0] == Root && pair[1] == node);
        }
    }

    private static List<int[]> ExpectedAncestorPairs()
    {
        var expected = new List<int[]>();

        for (var chain = 0; chain < ChainCount; chain++)
        {
            var chainStart = FirstChainNode + (chain * ChainLength);

            for (var offset = 0; offset < ChainLength; offset++)
            {
                for (var above = offset; above >= 0; above--)
                {
                    expected.Add([above == 0 ? Root : chainStart + above - 1, chainStart + offset]);
                }
            }
        }

        return expected;
    }

    private static int ChainOf(int node) => node == Root ? Root : FirstChainNode + (((node - FirstChainNode) / ChainLength) * ChainLength);
}
