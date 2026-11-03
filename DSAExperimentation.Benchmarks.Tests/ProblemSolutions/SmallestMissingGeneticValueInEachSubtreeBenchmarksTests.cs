using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestMissingGeneticValueInEachSubtreeBenchmarks (ARCHITECTURE 17.9).
// Arm agreement is BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the
// workload's shape. Setup builds the chain and its genetic values from a closed form, and the
// chain's values are decisive enough to check the baseline against directly: see the two answers
// below.
//
// Answers come back one per node in node order, and node i's mex belongs to node i, so
// AnswerGraphText.Of rather than OfUnordered keeps each mex scored against its own node.
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
    public void BruteForce_TwoHundredNodeChain_ReturnsEachNodesMex() =>
        Assert.Equal(AnswerGraphText.Of(ExpectedMexes()), AnswerGraphText.Of(BuildHarness().BruteForce()));

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
