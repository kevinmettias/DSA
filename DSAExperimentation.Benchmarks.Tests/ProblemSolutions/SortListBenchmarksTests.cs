using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortListBenchmarks (ARCHITECTURE 17.9): each arm is checked against what
// LeetCode 148's own contract makes decisive rather than only against the other: the returned list
// carries the input's values in ascending order.
//
// Setup is the oracle. It builds the chain 1 .. Length in ascending order and then shuffles it
// with a fixed seed, so the sorted answer is the ascending range itself and nothing about the
// shuffle has to be reconstructed. Each arm only reads the chain it is handed and rebuilds a
// fresh one, so one harness is safe to call twice in either order.
public sealed partial class SortListBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(ValuesOf(BuildHarness().MergeSortOverSequence())),
            AnswerGraphText.Of(ValuesOf(BuildHarness().MergeSortOverSequence())));

    [Fact]
    public void ArraySort_ShuffledAscendingRange_RebuildsTheRangeInOrder() =>
        Assert.Equal(
            AnswerGraphText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerGraphText.Of(ValuesOf(BuildHarness().ArraySort())));

    [Fact]
    public void MergeSortOverSequence_ShuffledAscendingRange_RebuildsTheRangeInOrder() =>
        Assert.Equal(
            AnswerGraphText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerGraphText.Of(ValuesOf(BuildHarness().MergeSortOverSequence())));

    private static SortListBenchmarks BuildHarness()
    {
        var harness = new SortListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static List<int> ValuesOf(SinglyLinkedListNode<int>? answer)
    {
        var values = new List<int>();

        for (var node = answer; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }
}
