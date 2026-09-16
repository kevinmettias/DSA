using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SwappingNodesInALinkedListBenchmarks (ARCHITECTURE 17.9): both arms are
// SwappingNodesInALinkedListSolution's - the array materialization against the fast/slow two-pointer
// walk - so a harness whose arms disagree is timing two different questions. Both arms return the
// internal node type through object, which renders as its own type name and would compare equal
// whatever the lists held, so each arm's answer is walked out into the value sequence first.
//
// The run is 1..Length and [GlobalSetup] fixes which position k picks, so the swapped run is decisive
// rather than merely agreed: LC 1721 exchanges the kth value from the front with the kth from the end,
// which the oracle below restates from the run and the position alone. Each arm builds its own list
// from the hoisted values, so one harness is safe to call twice in either order.
public sealed partial class SwappingNodesInALinkedListBenchmarksTests
{
    // Mirrors SwappingNodesInALinkedListBenchmarks' own private TargetIndexDivisor.
    private const int TargetIndexDivisor = 3;
    private const int SmallestLength = 200;
    private const int KthPosition = SmallestLength / TargetIndexDivisor;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayMaterializeSwap())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayMaterializeSwap())));

    [Fact]
    public void ArrayMaterializeSwap_RunWithOnePositionPair_AgreesWithTheOtherArmAndTheSwapRule()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.LinkedListTwoPointerSwap())),
            AnswerText.Of(ValuesOf(harness.ArrayMaterializeSwap())));
        Assert.Equal(
            AnswerText.Of(ExpectedSwappedValues()),
            AnswerText.Of(ValuesOf(harness.ArrayMaterializeSwap())));
    }

    [Fact]
    public void LinkedListTwoPointerSwap_RunWithOnePositionPair_AgreesWithTheOtherArmAndTheSwapRule()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.ArrayMaterializeSwap())),
            AnswerText.Of(ValuesOf(harness.LinkedListTwoPointerSwap())));
        Assert.Equal(
            AnswerText.Of(ExpectedSwappedValues()),
            AnswerText.Of(ValuesOf(harness.LinkedListTwoPointerSwap())));
    }

    private static SwappingNodesInALinkedListBenchmarks BuildHarness()
    {
        var harness = new SwappingNodesInALinkedListBenchmarks { Length = SmallestLength };
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

    // LC 1721 restated: the kth value of the run 1..Length is one with the kth counted back from the
    // end, so the values at 1-based positions k and Length - k + 1 trade places and nothing else moves.
    private static List<int> ExpectedSwappedValues()
    {
        var values = Enumerable.Range(1, SmallestLength).ToList();

        (values[KthPosition - 1], values[SmallestLength - KthPosition]) =
            (values[SmallestLength - KthPosition], values[KthPosition - 1]);

        return values;
    }
}
