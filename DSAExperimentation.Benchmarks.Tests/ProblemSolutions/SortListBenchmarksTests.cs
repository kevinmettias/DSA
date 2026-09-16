using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortListBenchmarks (ARCHITECTURE 17.9): the class has a single arm - the
// original benchmark's two [Benchmark] methods were an unimplemented placeholder returning the
// literal 1 - so there is no second strategy to reconcile it against and the assertion has to
// come from what LeetCode 148's own contract makes decisive instead: the returned list carries
// the input's values in ascending order.
//
// Setup is the oracle. It builds the chain 1 .. Length in ascending order and then shuffles it
// with a fixed seed, so the sorted answer is the ascending range itself and nothing about the
// shuffle has to be reconstructed. The arm only reads the chain it is handed and rebuilds a
// fresh one, so one harness is safe to call twice in either order.
public sealed partial class SortListBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().MergeSortOverSequence())),
            AnswerText.Of(ValuesOf(BuildHarness().MergeSortOverSequence())));

    [Fact]
    public void MergeSortOverSequence_ShuffledAscendingRange_RebuildsTheRangeInOrder() =>
        Assert.Equal(
            AnswerText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerText.Of(ValuesOf(BuildHarness().MergeSortOverSequence())));

    private static SortListBenchmarks BuildHarness()
    {
        var harness = new SortListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The arm reports the list as object, because the node type is internal, so the values are
    // read back through the chain rather than compared as a rendered answer.
    private static List<int> ValuesOf(object? answer)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)answer; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }
}
