using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestMissingGeneticValueInEachSubtreeBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - rescanning every node's whole
// subtree from scratch against one ancestor-chain walk - so a harness whose arms disagree is
// answering two different family trees. Setup builds the chain and its genetic values from a
// closed form with no draw from any stream, so the same NodeCount is the whole of what pins
// the workload.
//
// Answers come back one per node in node order, and node i's mex belongs to node i, so
// AnswerText.Of rather than OfUnorderedSet keeps each mex scored against its own node. The
// chain's values are also decisive enough to check the arms against directly rather than
// against each other: see the two answers below.
public sealed partial class SmallestMissingGeneticValueInEachSubtreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // Setup gives node 0 the value 2, node i the value i + 2, and the deepest leaf the value 1,
    // so the whole tree carries exactly the run 1 .. SmallestNodeCount and the root's subtree -
    // which is the whole tree - first misses the value just past it.
    private const int RootSubtreeAnswer = SmallestNodeCount + 1;

    // Every other node's subtree is a suffix of the chain: it still contains the leaf's 1, and
    // node 0 is the only node carrying a 2, so 2 is the first value missing from each of them.
    private const int ChainNodeAnswer = 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameFamilyTree() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_TwoHundredNodeChain_AgreesWithAncestorChainWithSkip()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.AncestorChainWithSkip()),
            AnswerText.Of(harness.BruteForce()));
        Assert.Equal(AnswerText.Of(ExpectedMexes()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void AncestorChainWithSkip_TwoHundredNodeChain_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.AncestorChainWithSkip()));
        Assert.Equal(AnswerText.Of(ExpectedMexes()), AnswerText.Of(harness.AncestorChainWithSkip()));
    }

    // The oracle Setup's own documentation settles: the root answers RootSubtreeAnswer and every
    // chain node after it answers ChainNodeAnswer.
    private static IEnumerable<int> ExpectedMexes() =>
        new[] { RootSubtreeAnswer }.Concat(Enumerable.Repeat(ChainNodeAnswer, SmallestNodeCount - 1));

    private static SmallestMissingGeneticValueInEachSubtreeBenchmarks BuildHarness()
    {
        var harness = new SmallestMissingGeneticValueInEachSubtreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
