using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.FindTheDuplicateNumber;

// LeetCode 287. Find the Duplicate Number: nums holds n+1 integers in [1, n], so at
// least one value repeats; find any one of them without modifying the array and
// using only O(1) extra space beyond the answer itself.
//
// Treating nums[i] as a pointer from node i to node nums[i] turns the array into an
// implicit linked list whose cycle entry is exactly the duplicate value - the same
// Floyd's-algorithm CycleDetection.FindCycleStart LinkedListCycleII already proves
// out, here driven by materializing real SinglyLinkedListNode<int> nodes.
internal static class FindTheDuplicateNumberSolution
{
    // The textbook answer: an O(n^2) all-pairs scan, deliberately written without
    // this repo's primitives - the arm the cycle-detection strategy below has to
    // justify itself against.
    public static int FindDuplicateByBruteForce(int[] nums)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] == nums[j])
                {
                    return nums[i];
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    // Materialize nums as real linked-list nodes - node i points to node nums[i] -
    // and hand the result to this repo's own Floyd's tortoise-and-hare. The meeting
    // point's entry node is the duplicate value, by the same reasoning that makes it
    // a cycle's start.
    public static int FindDuplicateByCycleDetection(int[] nums)
    {
        var nodes = new SinglyLinkedListNode<int>[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            nodes[i] = new SinglyLinkedListNode<int>(i);
        }

        for (var i = 0; i < nums.Length; i++)
        {
            nodes[i].Next = nodes[nums[i]];
        }

        return CycleDetection.FindCycleStart(nodes[0])!.Value;
    }
}
