using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeKSortedListsBenchmarks (ARCHITECTURE 17.9): the workload is its own
// oracle: list `offset` holds offset, ListCount + offset, 2 * ListCount + offset, ... so ListCount
// lists of ValuesPerList values tile 0 .. ListCount * ValuesPerList - 1 exactly once each, and the
// answer is documented to be that whole range in ascending order - an expected value the generic
// agreement check, which only compares arms with each other, cannot supply. The baseline reports the
// merged chain, so the test reads the values off it rather than comparing nodes; the merge is
// order-defining, so the order-sensitive rendering is the right comparison.
public sealed partial class MergeKSortedListsBenchmarksTests
{
    private const int SmallestListCount = 8;

    // Mirrors the harness's own per-list value count, which decides how far the tiled range runs.
    private const int ValuesPerList = 64;

    private const int ExpectedMergedValueCount = SmallestListCount * ValuesPerList;

    [Fact]
    public void FlattenSort_TiledValueLists_MergesIntoTheWholeAscendingRange() =>
        Assert.Equal(
            AnswerGraphText.Of(Enumerable.Range(0, ExpectedMergedValueCount)),
            AnswerGraphText.Of(Values(BuildHarness().FlattenSort())));

    // The arm reports the merged chain, so the test reads the values off it rather than comparing nodes.
    private static List<int> Values(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static MergeKSortedListsBenchmarks BuildHarness()
    {
        var harness = new MergeKSortedListsBenchmarks { ListCount = SmallestListCount };
        harness.Setup();

        return harness;
    }
}
