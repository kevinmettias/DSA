using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveNthNodeFromEndOfListBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveNthNodeFromEndOfListSolution's, competing strategies for the same question - rebuilding the
// chain from values against one two-runner pass - so a harness whose arms disagree returns a
// different chain. Each arm builds the list fresh inside the call, and both derive the removed
// position from Length the same way, so the hoisted _values is never written through and one harness
// serves both arms in either order. Both arms return the head as object (the node type is internal,
// so a public [Benchmark] method cannot name it as a return type, CS0050), which AnswerText cannot
// render - a linked node is not an enumerable sequence - so each result is walked into its values
// first.
public sealed partial class RemoveNthNodeFromEndOfListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRebuild())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRebuild())));

    [Fact]
    public void ArrayRebuild_AgreesWithTwoRunner()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.TwoRunner())),
            AnswerText.Of(ValuesOf(harness.ArrayRebuild())));
    }

    [Fact]
    public void TwoRunner_AgreesWithArrayRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.ArrayRebuild())),
            AnswerText.Of(ValuesOf(harness.TwoRunner())));
    }

    private static RemoveNthNodeFromEndOfListBenchmarks BuildHarness()
    {
        var harness = new RemoveNthNodeFromEndOfListBenchmarks { Length = SmallestLength };
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
