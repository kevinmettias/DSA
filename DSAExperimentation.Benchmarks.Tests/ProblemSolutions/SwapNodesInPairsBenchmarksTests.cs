using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SwapNodesInPairsBenchmarks (ARCHITECTURE 17.9): both arms are
// SwapNodesInPairsSolution's - the in-place pointer rewiring against the array round trip - so a
// harness whose arms disagree is timing two different questions. Both arms return the internal node
// type through object, which renders as its own type name and would compare equal whatever the lists
// held, so each arm's answer is walked out into the value sequence first.
//
// The run is 1..Length, whose adjacent pairs are decisive rather than merely agreed: every pair
// exchanges places and Length is even, so the swapped run is stated by the oracle below rather than
// read back out of an arm. Both arms build their own list from the hoisted values, so one harness is
// safe to call twice in either order.
public sealed partial class SwapNodesInPairsBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int PairStride = 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRoundTrip())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRoundTrip())));

    [Fact]
    public void ArrayRoundTrip_PairedRun_AgreesWithTheOtherArmAndThePairSwapRule()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.PointerRewiring())),
            AnswerText.Of(ValuesOf(harness.ArrayRoundTrip())));
        Assert.Equal(
            AnswerText.Of(ExpectedSwappedValues()),
            AnswerText.Of(ValuesOf(harness.ArrayRoundTrip())));
    }

    [Fact]
    public void PointerRewiring_PairedRun_AgreesWithTheOtherArmAndThePairSwapRule()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.ArrayRoundTrip())),
            AnswerText.Of(ValuesOf(harness.PointerRewiring())));
        Assert.Equal(
            AnswerText.Of(ExpectedSwappedValues()),
            AnswerText.Of(ValuesOf(harness.PointerRewiring())));
    }

    private static SwapNodesInPairsBenchmarks BuildHarness()
    {
        var harness = new SwapNodesInPairsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static List<int> ValuesOf(object? head)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)head; node is not null; node = node.Next)
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
