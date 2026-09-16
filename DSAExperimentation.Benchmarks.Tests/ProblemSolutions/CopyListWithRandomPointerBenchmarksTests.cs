using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CopyListWithRandomPointerBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile it against and the assertion has to come
// from what LeetCode 138's own contract makes decisive instead - the clone's Random must resolve to
// the clone of whatever the original pointed at, never to the original node itself. Every Random
// reachable from the returned clone must therefore be one of that clone's own nodes and nothing
// else, which is what the class comment's "references pointing both forward and backward" shape
// exists to exercise. Setup draws from one fixed seed, so the same NodeCount must rebuild the same
// list, asserted through the one observable the arm exposes, its clone.
public sealed partial class CopyListWithRandomPointerBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // What the rendering writes where a node's Random resolves to nothing.
    private const string NullRandomText = "null";

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(SmallestNodeCount, Chain(Copied()).Count);

        Assert.Equal(Render(Copied()), Render(Copied()));
    }

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
        (RandomLinkedListNode<int>?)BuildHarness().HashMapMemo();

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

    // One entry per node, naming the node it is reached from and the value its Random resolves to.
    // Rendering the clone this way compares two builds without caring about node identity, which is
    // the only thing two separate clones never share.
    private static string Render(RandomLinkedListNode<int>? clone) =>
        AnswerText.Of(Chain(clone).Select(node =>
            $"{node.Value}:{(node.Random is null ? NullRandomText : node.Random.Value.ToString())}"));
}
