using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindEventualSafeStatesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a per-node three-color DFS against modeling the
// same question as Kahn's algorithm over the reversed graph - so a harness whose arms disagree is
// peeling two different graphs. Both arms return the safe ids ascending, which is the order the
// problem fixes, so they are compared as ordered sequences.
//
// Setup splits the nodes in half: [0, half) is a forward DAG whose every node reaches the terminal
// node half - 1 (its own fan-out shrinks to nothing there), and [half, NodeCount) is one ring cycle
// whose every node loops forever. Every node in the first half is therefore safe and every node in
// the second is not, which makes the answer decisive rather than a happy accident of the seed.
public sealed partial class FindEventualSafeStatesBenchmarksTests
{
    // The smaller of Setup's [Params(50, 1_000)] node counts.
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameSafeNodeList() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DfsThreeColoring()),
            AnswerText.Of(BuildHarness().DfsThreeColoring()));

    [Fact]
    public void DfsThreeColoring_SafeForwardHalfAndRingCycle_ReturnsTheForwardHalfAscending()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ExpectedSafeNodes()), AnswerText.Of(harness.DfsThreeColoring()));
        Assert.Equal(
            AnswerText.Of(harness.ReversedKahnsTopologicalSort()),
            AnswerText.Of(harness.DfsThreeColoring()));
    }

    [Fact]
    public void ReversedKahnsTopologicalSort_SafeForwardHalfAndRingCycle_AgreesWithDfsThreeColoring()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ExpectedSafeNodes()), AnswerText.Of(harness.ReversedKahnsTopologicalSort()));
        Assert.Equal(
            AnswerText.Of(harness.DfsThreeColoring()),
            AnswerText.Of(harness.ReversedKahnsTopologicalSort()));
    }

    private static FindEventualSafeStatesBenchmarks BuildHarness()
    {
        var harness = new FindEventualSafeStatesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // Setup's first half, read off its own layout: node i points only at higher ids inside
    // [0, half), so the walk always terminates at the node whose fan-out shrinks to zero. The
    // second half is a single ring and never peels, so no id at or above half can be safe.
    private static int[] ExpectedSafeNodes() =>
        Enumerable.Range(0, SmallestNodeCount / AlgorithmConstants.HalvingFactor).ToArray();
}
