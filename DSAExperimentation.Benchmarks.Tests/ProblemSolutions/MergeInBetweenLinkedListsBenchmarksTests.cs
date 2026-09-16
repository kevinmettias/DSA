using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeInBetweenLinkedListsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - copying the list values into an array and rebuilding the
// spliced chain against rewiring the original nodes' Next pointers in place - so a harness whose arms
// disagree is timing two different problems. Both arms report the resulting chain as an object over the
// data structure's internal node type, so the test reads the values off it rather than comparing nodes.
//
// The splice replaces the window's nodes with the second list's, so the answer is documented to hold
// exactly as many nodes as the first list started with, and that count is what the arm tests pin before
// comparing the two readings. Both arms rebuild their input lists inside the measured call, so one
// harness is safe to read twice in either order. Setup derives the values and the window from Length
// alone, so the same Length must rebuild the same splice.
public sealed partial class MergeInBetweenLinkedListsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Five nodes leave with the window and five arrive with the second list, so the merged chain is
    // exactly as long as list1 was.
    private const int ExpectedMergedNodeCount = SmallestLength;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(Values(BuildHarness().ArraySpliceRebuild())),
            AnswerText.Of(Values(BuildHarness().ArraySpliceRebuild())));

    [Fact]
    public void ArraySpliceRebuild_SeededLists_AgreesWithLinkedListSplice()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMergedNodeCount, Values(harness.ArraySpliceRebuild()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.LinkedListSplice())),
            AnswerText.Of(Values(harness.ArraySpliceRebuild())));
    }

    [Fact]
    public void LinkedListSplice_SeededLists_AgreesWithArraySpliceRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMergedNodeCount, Values(harness.LinkedListSplice()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.ArraySpliceRebuild())),
            AnswerText.Of(Values(harness.LinkedListSplice())));
    }

    // Both arms report the merged chain as that internal node type through an object, so the test reads
    // the values off it rather than comparing nodes.
    private static List<int> Values(object head)
    {
        var values = new List<int>();

        for (var node = head as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static MergeInBetweenLinkedListsBenchmarks BuildHarness()
    {
        var harness = new MergeInBetweenLinkedListsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
