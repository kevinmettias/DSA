using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CircularArrayLoop;

// LeetCode 457. Circular Array Loop: reframe "jump to (i + nums[i]) mod n" as a
// functional graph of SinglyLinkedListNode<int> links built once up front (an edge is
// left null - dead end - wherever it would be a same-index self loop or a direction
// change between adjacent indices, since both disqualify a LeetCode cycle), then reuse
// this repo's own Floyd's tortoise-and-hare CycleDetection.HasCycle to look for a
// cycle starting from every index.
public sealed partial class CircularArrayLoopTests
{
    [Fact]
    public void HasValidCycle_UniformForwardSteps_FindsFullArrayCycle()
    {
        int[] nums = [1, 1, 1, 1];

        Assert.True(HasValidCycle(nums));
    }

    [Fact]
    public void HasValidCycle_ClassicExample_FindsThreeIndexCycle()
    {
        int[] nums = [2, -1, 1, 2, 2];

        Assert.True(HasValidCycle(nums));
    }

    [Fact]
    public void HasValidCycle_OnlyReachableCycleIsSelfLoop_ReturnsFalse()
    {
        int[] nums = [-1, -2, -3, -4, -5, 6];

        Assert.False(HasValidCycle(nums));
    }

    [Fact]
    public void HasValidCycle_AlternatingDirectionsOnly_ReturnsFalse()
    {
        int[] nums = [1, -1, 1, -1];

        Assert.False(HasValidCycle(nums));
    }

    private static bool HasValidCycle(int[] nums)
    {
        var nodes = BuildNodes(nums.Length);
        LinkEdges(nodes, nums);
        return AnyCycle(nodes);
    }

    private static SinglyLinkedListNode<int>[] BuildNodes(int n)
    {
        var nodes = new SinglyLinkedListNode<int>[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new SinglyLinkedListNode<int>(i);
        }

        return nodes;
    }

    private static void LinkEdges(SinglyLinkedListNode<int>[] nodes, int[] nums)
    {
        var n = nums.Length;

        for (var i = 0; i < n; i++)
        {
            var nextIndex = (((i + nums[i]) % n) + n) % n;

            if (nextIndex == i || Math.Sign(nums[nextIndex]) != Math.Sign(nums[i]))
            {
                continue;
            }

            nodes[i].Next = nodes[nextIndex];
        }
    }

    private static bool AnyCycle(SinglyLinkedListNode<int>[] nodes)
    {
        foreach (var node in nodes)
        {
            if (CycleDetection.HasCycle(node))
            {
                return true;
            }
        }

        return false;
    }
}
