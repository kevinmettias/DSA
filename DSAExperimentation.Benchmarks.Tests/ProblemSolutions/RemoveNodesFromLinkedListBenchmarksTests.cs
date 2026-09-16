using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveNodesFromLinkedListBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveNodesFromLinkedListSolution's, competing strategies for the same question - a rescan from
// every node against one monotonic-stack sweep - so a harness whose arms disagree returns a different
// chain. Only the value permutation is hoisted, and each arm builds the chain inside the call, so the
// hoisted _values is never written through and one harness serves both arms in either order. Both
// arms return the head as object (the node type is internal, so a public [Benchmark] method cannot
// name it as a return type, CS0050), which AnswerText cannot render - a linked node is not an
// enumerable sequence - so each result is walked into its values first.
public sealed partial class RemoveNodesFromLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().BruteForceScan())),
            AnswerText.Of(ValuesOf(BuildHarness().BruteForceScan())));

    [Fact]
    public void BruteForceScan_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.MonotonicStackSweep())),
            AnswerText.Of(ValuesOf(harness.BruteForceScan())));
    }

    [Fact]
    public void MonotonicStackSweep_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.BruteForceScan())),
            AnswerText.Of(ValuesOf(harness.MonotonicStackSweep())));
    }

    private static RemoveNodesFromLinkedListBenchmarks BuildHarness()
    {
        var harness = new RemoveNodesFromLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static int[] ValuesOf(object? answer)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)answer; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }
}
