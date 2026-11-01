using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CopyListWithRandomPointerBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile it against and the assertion has to come
// from what LeetCode 138's own contract makes decisive instead - the clone's Random must resolve to
// the clone of whatever the original pointed at, never to the original node itself. Every Random
// reachable from the returned clone must therefore be one of that clone's own nodes and nothing
// else, which is what the class comment's "references pointing both forward and backward" shape
// exists to exercise.
public sealed partial class CopyListWithRandomPointerBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void HashMapMemo_TwoHundredNodeList_ReturnsADeepCopyWhoseRandomsStayInsideIt()
    {
        var nodes = Chain(Copied());

        Assert.Equal(Enumerable.Range(0, SmallestNodeCount), nodes.Select(node => node.Value));
        Assert.Contains(nodes, node => node.Random is not null);

        foreach (var node in nodes)
        {
            if (node.Random is not null)
            {
                Assert.Contains(node.Random, nodes);
            }
        }
    }

    private static RandomLinkedListNode<int>? Copied() =>
        BuildHarness().HashMapMemo();

    private static CopyListWithRandomPointerBenchmarks BuildHarness()
    {
        var harness = new CopyListWithRandomPointerBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    private static List<RandomLinkedListNode<int>> Chain(RandomLinkedListNode<int>? head)
    {
        var nodes = new List<RandomLinkedListNode<int>>();

        for (var node = head; node is not null; node = node.Next)
        {
            nodes.Add(node);
        }

        return nodes;
    }
}
