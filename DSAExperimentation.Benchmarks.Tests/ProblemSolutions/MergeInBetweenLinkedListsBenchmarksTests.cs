using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeInBetweenLinkedListsBenchmarks (ARCHITECTURE 17.9): the splice replaces
// the window's nodes with the second list's, so the answer is documented to hold exactly as many nodes
// as the first list started with - an expected value derived from the workload's construction rather
// than from either arm, which the generic agreement check cannot supply. The baseline reports the
// resulting chain, so the test reads the values off it rather than comparing nodes.
public sealed partial class MergeInBetweenLinkedListsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Five nodes leave with the window and five arrive with the second list, so the merged chain is
    // exactly as long as list1 was.
    private const int ExpectedMergedNodeCount = SmallestLength;

    [Fact]
    public void ArraySpliceRebuild_SeededLists_KeepsTheFirstListsLength() =>
        Assert.Equal(ExpectedMergedNodeCount, Values(BuildHarness().ArraySpliceRebuild()).Count);

    // The arm reports the merged chain, so the test reads the values off it rather than comparing nodes.
    private static List<int> Values(SinglyLinkedListNode<int> head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
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
