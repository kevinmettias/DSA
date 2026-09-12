using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.CircularArrayLoop;

// LeetCode 457. Circular Array Loop: does there exist a start index whose
// "jump to (i + nums[i]) mod n" chain forms a cycle of length > 1 that never
// changes direction (all-positive or all-negative steps throughout)?
//
// The baseline walks each start with a fresh HashSet<int>, checking the
// self-loop/direction-change disqualifiers on the fly - the arm the composed
// solution has to justify itself against. The composed solution instead
// reframes the jump rule as a functional graph of SinglyLinkedListNode<int>
// links, built once up front (an edge is left null wherever it would be a
// same-index self loop or a direction change between adjacent indices, since
// both disqualify a LeetCode cycle), then reuses this repo's own Floyd's
// tortoise-and-hare CycleDetection.HasCycle to look for a cycle from every
// index.
internal static class CircularArrayLoopSolution
{
    // The textbook per-start walk, deliberately written without this repo's
    // primitives.
    public static bool HasLoopByHashSetWalk(int[] nums)
    {
        for (var start = 0; start < nums.Length; start++)
        {
            if (HasCycleFrom(nums, start))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasCycleFrom(int[] nums, int start)
    {
        var n = nums.Length;
        var visited = new HashSet<int>();
        var current = start;

        while (visited.Add(current))
        {
            var next = (((current + nums[current]) % n) + n) % n;

            if (next == current || Math.Sign(nums[next]) != Math.Sign(nums[current]))
            {
                return false;
            }

            current = next;
        }

        return true;
    }

    public static bool HasLoopByLinkedListFloyd(int[] nums)
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
