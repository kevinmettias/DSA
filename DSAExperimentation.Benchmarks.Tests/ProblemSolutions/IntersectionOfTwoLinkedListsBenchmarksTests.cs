using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.IntersectionOfTwoLinkedLists;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IntersectionOfTwoLinkedListsBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm - the two-pointer walk - so there is no second strategy to reconcile it against and
// the assertion has to come from the arm's own declared contract instead. The arm returns the
// converging node itself, null when the walk finds nothing, and the fixture
// IntersectionOfTwoLinkedListsWorkloads.Build makes both chains converge onto one shared tail node
// whose value is -1 and which has no successor, so a walk that fell off both lists fails the type
// check before its value is read. The decisive oracle beside it is reference identity - the fixture
// places the shared node at index PrefixLength of the first chain and at index 2 * PrefixLength of
// the second - and each [Fact] asserts the walk lands on that exact node, which no fallback can
// counterfeit. Chain construction is charged to [GlobalSetup], so one harness is safe to call twice
// in either order.
public sealed partial class IntersectionOfTwoLinkedListsBenchmarksTests
{
    private const int SmallestPrefixLength = 100;

    // The value of the shared node the fixture appends both chains to.
    private const int SharedNodeValue = -1;

    [Fact]
    public void Setup_SamePrefixLength_RebuildsTheSameChains()
    {
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().TwoPointerWalk()),
            AnswerGraphText.Of(BuildHarness().TwoPointerWalk()));

        var (firstHead, secondHead) = IntersectionOfTwoLinkedListsWorkloads.Build(SmallestPrefixLength);

        Assert.Same(
            NodeAt(firstHead, SmallestPrefixLength),
            IntersectionOfTwoLinkedListsSolution.GetIntersectionNodeByTwoPointerWalk(firstHead, secondHead));
    }

    [Fact]
    public void TwoPointerWalk_ConvergentChains_ReturnsTheSharedTailNode()
    {
        var found = Assert.IsType<SinglyLinkedListNode<int>>(BuildHarness().TwoPointerWalk());

        Assert.Equal(SharedNodeValue, found.Value);
        Assert.Null(found.Next);

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
