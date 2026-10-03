using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OddEvenLinkedListBenchmarks (ARCHITECTURE 17.9): both arms regroup the same
// chain - one rebuilds it from two value buffers, the other rewires the existing nodes in place - and
// return the regrouped list's head. Setup builds the chain 0..Length-1 once and each arm clones it
// before mutating, so one harness is safe to call twice in either order.
//
// Because node i holds the value i, the regrouped list is derivable from Length alone: the nodes at
// odd positions (values 0, 2, 4, ...) in their original order, then the even positions (1, 3, 5, ...).
// Each arm's list is checked against that, which catches a dropped or duplicated node and a parity
// grouped the wrong way alike.
public sealed partial class OddEvenLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // LC 328 numbers positions from one, so the first node is odd.
    private const int PositionParity = 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().TwoListRebuild()),
            AnswerGraphText.Of(BuildHarness().TwoListRebuild()));

    [Fact]
    public void TwoListRebuild_SmallestLength_KeepsEveryNode() =>
        Assert.Equal(ExpectedValues(), ValuesOf(BuildHarness().TwoListRebuild()));

    [Fact]
    public void InPlaceRewire_SmallestLength_KeepsEveryNode() =>
        Assert.Equal(ExpectedValues(), ValuesOf(BuildHarness().InPlaceRewire()));

    private static int[] ExpectedValues()
    {
        var values = Enumerable.Range(0, SmallestLength).ToArray();

        return [.. values.Where(value => value % PositionParity == 0), .. values.Where(value => value % PositionParity != 0)];
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

    private static OddEvenLinkedListBenchmarks BuildHarness()
    {
        var harness = new OddEvenLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
