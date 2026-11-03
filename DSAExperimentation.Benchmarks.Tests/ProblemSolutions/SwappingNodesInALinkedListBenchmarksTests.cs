using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SwappingNodesInALinkedListBenchmarks (ARCHITECTURE 17.9): the node at 1-based
// position p holds p mod 101 and [GlobalSetup] fixes which position k picks, and at the smallest
// Length the two swapped positions hold different values, so the swapped run is decisive rather than
// merely agreed: LC 1721 exchanges the kth value from the front with the kth from the end, which the
// oracle below restates from the run and the position alone. The baseline is held to it; the generic
// arm check holds the two-pointer arm to the baseline's answer.
public sealed partial class SwappingNodesInALinkedListBenchmarksTests
{
    // Mirrors SwappingNodesInALinkedListBenchmarks' own private TargetIndexDivisor.
    private const int TargetIndexDivisor = 3;

    // Mirrors SwappingNodesInALinkedListBenchmarks' own private NodeValueSpan.
    private const int NodeValueSpan = 101;
    private const int SmallestLength = 200;
    private const int KthPosition = SmallestLength / TargetIndexDivisor;

    [Fact]
    public void ArrayMaterializeSwap_RunWithOnePositionPair_FollowsTheSwapRule() =>
        Assert.Equal(
            AnswerGraphText.Of(ExpectedSwappedValues()),
            AnswerGraphText.Of(ValuesOf(BuildHarness().ArrayMaterializeSwap())));

    private static SwappingNodesInALinkedListBenchmarks BuildHarness()
    {
        var harness = new SwappingNodesInALinkedListBenchmarks { Length = SmallestLength };
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

    // LC 1721 restated: the kth value of the run is one with the kth counted back from the end, so the
    // values at 1-based positions k and Length - k + 1 trade places and nothing else moves.
    private static List<int> ExpectedSwappedValues()
    {
        var values = Enumerable.Range(1, SmallestLength).Select(position => position % NodeValueSpan).ToList();

        (values[KthPosition - 1], values[SmallestLength - KthPosition]) =
            (values[SmallestLength - KthPosition], values[KthPosition - 1]);

        return values;
    }
}
