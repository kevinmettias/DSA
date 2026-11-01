using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OddEvenLinkedListBenchmarks (ARCHITECTURE 17.9): the baseline's regrouped list
// against one derived from Length rather than from either arm, which the generic check cannot supply
// - it holds the in-place arm to the baseline. Setup builds the chain 0..Length-1, and because node i
// holds the value i, the regrouped list is derivable from Length alone: the nodes at odd positions
// (values 0, 2, 4, ...) in their original order, then the even positions (1, 3, 5, ...). That catches
// a dropped or duplicated node and a parity grouped the wrong way alike.
public sealed partial class OddEvenLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // LC 328 numbers positions from one, so the first node is odd.
    private const int PositionParity = 2;

    [Fact]
    public void TwoListRebuild_SmallestLength_KeepsEveryNode() =>
        Assert.Equal(ExpectedValues(), ValuesOf(BuildHarness().TwoListRebuild()));

    private static int[] ExpectedValues()
    {
        var values = Enumerable.Range(0, SmallestLength).ToArray();

        return [.. values.Where(value => value % PositionParity == 0), .. values.Where(value => value % PositionParity != 0)];
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

    private static OddEvenLinkedListBenchmarks BuildHarness()
    {
        var harness = new OddEvenLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
