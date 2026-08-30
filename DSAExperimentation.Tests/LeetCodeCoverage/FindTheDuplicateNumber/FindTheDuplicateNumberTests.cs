using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheDuplicateNumber;

// LeetCode 287. Find the Duplicate Number: treating nums[i] as a pointer from node i
// to node nums[i] turns the array into an implicit linked list whose cycle entry is
// exactly the duplicate value - materialize that list as real SinglyLinkedListNode<int>
// nodes and hand it to this repo's own Floyd's-algorithm CycleDetection.FindCycleStart,
// the same primitive LinkedListCycleII already proves out.
public sealed partial class FindTheDuplicateNumberTests
{
    [Fact]
    public void FindDuplicate_ClassicExample_ReturnsRepeatedValue()
    {
        int[] nums = [1, 3, 4, 2, 2];

        Assert.Equal(2, FindDuplicate(nums));
    }

    [Fact]
    public void FindDuplicate_ValueRepeatedMultipleTimes_ReturnsRepeatedValue()
    {
        int[] nums = [3, 1, 3, 4, 2];

        Assert.Equal(3, FindDuplicate(nums));
    }

    private static int FindDuplicate(int[] nums)
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
