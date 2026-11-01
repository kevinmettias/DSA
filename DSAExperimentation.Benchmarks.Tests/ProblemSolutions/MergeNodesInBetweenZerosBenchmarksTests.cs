using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeNodesInBetweenZerosBenchmarks (ARCHITECTURE 17.9): the generated list is
// a leading zero, GroupCount runs of one to four non-zero values each closed by a zero, so the answer
// is documented to be exactly one sum node per run - GroupCount nodes - an expected value derived
// from the workload's construction rather than from either arm, which the generic agreement check
// cannot supply. The baseline reports the merged chain, so the test reads the values off it rather
// than comparing nodes.
public sealed partial class MergeNodesInBetweenZerosBenchmarksTests
{
    private const int SmallestGroupCount = 200;

    private const int ExpectedMergedNodeCount = SmallestGroupCount;

    [Fact]
    public void TwoPassValueBuffer_DelimitedSeededList_SumsEachRunIntoOneNode() =>
        Assert.Equal(ExpectedMergedNodeCount, Values(BuildHarness().TwoPassValueBuffer()).Count);

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

    private static MergeNodesInBetweenZerosBenchmarks BuildHarness()
    {
        var harness = new MergeNodesInBetweenZerosBenchmarks { GroupCount = SmallestGroupCount };
        harness.Setup();

        return harness;
    }
}
