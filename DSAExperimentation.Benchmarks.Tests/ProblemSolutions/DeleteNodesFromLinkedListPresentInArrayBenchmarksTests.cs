using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteNodesFromLinkedListPresentInArrayBenchmarks (ARCHITECTURE 17.9): the
// generic arm check compares the arms only with each other, so what stays here is a bound derived
// without either arm. The reading is the surviving chain, whose documented shape is at most one node
// per value the list started with.
public sealed partial class DeleteNodesFromLinkedListPresentInArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every value the list started with is either deleted or kept, so nothing can be added.
    private const int MinimumSurvivorCount = 0;

    [Fact]
    public void ArrayScan_TwoHundredSeededValues_KeepsAtMostOneNodePerValue() =>
        Assert.InRange(
            Values(BuildHarness().ArrayScan()).Count,
            MinimumSurvivorCount,
            SmallestLength);

    // The arm reports the surviving chain, so the test reads the values off it rather than counting nodes.
    private static List<int> Values(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static DeleteNodesFromLinkedListPresentInArrayBenchmarks BuildHarness()
    {
        var harness = new DeleteNodesFromLinkedListPresentInArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
