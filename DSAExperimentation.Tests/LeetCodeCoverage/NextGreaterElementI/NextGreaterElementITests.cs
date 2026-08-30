using DSAExperimentation.DataStructures.HashMap;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterElementI;

// LeetCode 496. Next Greater Element I: a monotonic decreasing Stack<int> walk over
// nums2 (MinStackTests/EvaluateReversePolishNotationTests precedent for this repo's own
// Stack) builds each value's next-greater in one O(n) pass, recorded into this repo's own
// HashMap<int,int>; nums1's answers are then just O(1) lookups into that map.
public sealed partial class NextGreaterElementITests
{
    [Fact]
    public void NextGreaterElement_ClassicExampleOne_ReturnsPerElementAnswers()
    {
        int[] nums1 = [4, 1, 2];
        int[] nums2 = [1, 3, 4, 2];

        var result = NextGreaterElement(nums1, nums2);

        Assert.Equal([-1, 3, -1], result);
    }

    [Fact]
    public void NextGreaterElement_ClassicExampleTwo_ReturnsPerElementAnswers()
    {
        int[] nums1 = [2, 4];
        int[] nums2 = [1, 2, 3, 4];

        var result = NextGreaterElement(nums1, nums2);

        Assert.Equal([3, -1], result);
    }

    [Fact]
    public void NextGreaterElement_NoElementHasAGreaterFollower_ReturnsAllNegativeOne()
    {
        int[] nums1 = [4, 3];
        int[] nums2 = [4, 3, 2, 1];

        var result = NextGreaterElement(nums1, nums2);

        Assert.Equal([-1, -1], result);
    }

    private static int[] NextGreaterElement(int[] nums1, int[] nums2)
    {
        var nextGreater = new HashMap<int, int>();
        var decreasing = new RepoIntStack();

        foreach (var value in nums2)
        {
            while (decreasing.TryPeek(out var top) && top < value)
            {
                decreasing.TryPop(out _);
                nextGreater.Set(top, value);
            }

            decreasing.Push(value);
        }

        var result = new int[nums1.Length];

        for (var i = 0; i < nums1.Length; i++)
        {
            result[i] = nextGreater.TryGetValue(nums1[i], out var greater) ? greater : -1;
        }

        return result;
    }
}
