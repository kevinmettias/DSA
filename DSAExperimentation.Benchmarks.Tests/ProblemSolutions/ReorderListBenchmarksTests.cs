using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReorderListBenchmarks (ARCHITECTURE 17.9): the class carries a single arm - the
// reverse-and-merge reorder - so there is no second strategy to reconcile it against and the assertion
// has to come from the arm's own declared contract instead. That contract is decisive: reordering the
// chain L0 -> ... -> Ln-1 as L0 -> Ln-1 -> L1 -> Ln-2 -> ... has exactly one answer for the sequential
// values Setup builds, and the oracle below derives that answer from the position pattern alone, never
// from anything the arm produced. The arm returns the head as object (the node type is internal, so a
// public [Benchmark] method cannot name it as a return type, CS0050), which AnswerText cannot render -
// a linked node is not an enumerable sequence - so the result is walked into its values first.
public sealed partial class ReorderListBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ReverseAndMergeInPlace())),
            AnswerText.Of(ValuesOf(BuildHarness().ReverseAndMergeInPlace())));

    [Fact]
    public void ReverseAndMergeInPlace_SequentialValues_WeavesTheChainEndsInward()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(EndsInwardOracle()),
            AnswerText.Of(ValuesOf(harness.ReverseAndMergeInPlace())));
    }

    private static ReorderListBenchmarks BuildHarness()
    {
        var harness = new ReorderListBenchmarks { Length = SmallestLength };
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

    // Setup values the chain 1..Length; the reorder takes the next unused value from the front, then
    // the next unused value from the back, until the two ends meet.
    private static int[] EndsInwardOracle()
    {
        var reordered = new int[SmallestLength];
        var front = 0;
        var back = SmallestLength - 1;
        var index = 0;

        while (front <= back)
        {
            reordered[index++] = front + 1;

            if (front < back)
            {
                reordered[index++] = back + 1;
            }

            front++;
            back--;
        }

        return reordered;
    }
}
