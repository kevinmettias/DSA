using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SwapNodesInPairsBenchmarks (ARCHITECTURE 17.9): the run is 1..Length, whose
// adjacent pairs are decisive rather than merely agreed: every pair exchanges places and Length is
// even, so the swapped run is stated by the oracle below rather than read back out of an arm. The
// baseline is held to it; the generic arm check holds the pointer-rewiring arm to the baseline's
// answer.
public sealed partial class SwapNodesInPairsBenchmarksTests
{
    private const int SmallestLength = 10;
    private const int PairStride = 2;

    [Fact]
    public void ArrayRoundTrip_PairedRun_FollowsThePairSwapRule() =>
        Assert.Equal(
            AnswerGraphText.Of(ExpectedSwappedValues()),
            AnswerGraphText.Of(ValuesOf(BuildHarness().ArrayRoundTrip())));

    private static SwapNodesInPairsBenchmarks BuildHarness()
    {
        var harness = new SwapNodesInPairsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static List<int> ValuesOf(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    // LC 24 restated: every adjacent pair of the run 1..Length exchanges places, and with an even
    // Length no node is left over.
    private static List<int> ExpectedSwappedValues() =>
        Enumerable.Range(1, SmallestLength)
            .Chunk(PairStride)
            .SelectMany(pair => pair.Reverse())
            .ToList();
}
