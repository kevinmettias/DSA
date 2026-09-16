using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.IntersectionOfTwoLinkedLists;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IntersectionOfTwoLinkedListsBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm - the two-pointer walk - so there is no second strategy to reconcile it against and
// the assertion has to come from the arm's own declared contract instead. The arm reports the
// converging node's value, or the sentinel -1 when the walk finds nothing, and the fixture
// IntersectionOfTwoLinkedListsWorkloads.Build makes both converge onto one shared tail node whose
// value is that same -1. That makes the arm's number ambiguous on its own: a walk that found the
// shared node and a walk that fell off both lists report alike. So the decisive oracle here is
// reference identity - the fixture places the shared node at index PrefixLength of the first chain
// and at index 2 * PrefixLength of the second - and each [Fact] asserts the walk lands on that
// exact node, which no fallback can counterfeit. Chain construction is charged to [GlobalSetup], so
// one harness is safe to call twice in either order.
public sealed partial class IntersectionOfTwoLinkedListsBenchmarksTests
{
    private const int SmallestPrefixLength = 100;

    // The shared node the fixture appends both chains to, and both the arm's and the fixture's
    // not-found sentinel.
    private const int SharedNodeValue = -1;

    [Fact]
    public void Setup_SamePrefixLength_RebuildsTheSameChains()
    {
        Assert.Equal(BuildHarness().TwoPointerWalk(), BuildHarness().TwoPointerWalk());

        var (firstHead, secondHead) = IntersectionOfTwoLinkedListsWorkloads.Build(SmallestPrefixLength);

        Assert.Same(
            NodeAt(firstHead, SmallestPrefixLength),
            IntersectionOfTwoLinkedListsSolution.GetIntersectionNodeByTwoPointerWalk(firstHead, secondHead));
    }

    [Fact]
    public void TwoPointerWalk_ConvergentChains_ReturnsTheSharedTailNode()
    {
        Assert.Equal(SharedNodeValue, BuildHarness().TwoPointerWalk());

        var (firstHead, secondHead) = IntersectionOfTwoLinkedListsWorkloads.Build(SmallestPrefixLength);

        Assert.Same(
            NodeAt(firstHead, SmallestPrefixLength),
            IntersectionOfTwoLinkedListsSolution.GetIntersectionNodeByTwoPointerWalk(firstHead, secondHead));
    }

    private static IntersectionOfTwoLinkedListsBenchmarks BuildHarness()
    {
        var harness = new IntersectionOfTwoLinkedListsBenchmarks { PrefixLength = SmallestPrefixLength };
        harness.Setup();

        return harness;
    }

    private static SinglyLinkedListNode<int> NodeAt(SinglyLinkedListNode<int> head, int index)
    {
        var node = head;

        for (var step = 0; step < index; step++)
        {
            var next = node.Next;
            Assert.NotNull(next);

            node = next;
        }

        return node;
    }
}
