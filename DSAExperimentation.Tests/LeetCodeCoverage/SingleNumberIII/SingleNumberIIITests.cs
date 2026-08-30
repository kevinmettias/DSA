using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SingleNumberIII;

// LeetCode 260. Single Number III: every element appears exactly twice
// except two, which appear exactly once. A HashMap<int,int> frequency count
// isolates them in one O(n) pass - the same lookup-table technique
// TwoSumTests already uses, just counting occurrences instead of indices.
public sealed partial class SingleNumberIIITests
{
    [Fact]
    public void FindSingleNumbers_ClassicExample_ReturnsBothUniqueValues()
    {
        int[] nums = [1, 2, 1, 3, 2, 5];

        var found = FindTwoSingleNumbers(nums);

        Assert.Equal([3, 5], found.OrderBy(x => x));
    }

    [Fact]
    public void FindSingleNumbers_TwoElementsOnly_ReturnsBoth()
    {
        int[] nums = [-1, 0];

        var found = FindTwoSingleNumbers(nums);

        Assert.Equal([-1, 0], found.OrderBy(x => x));
    }

    private static int[] FindTwoSingleNumbers(int[] nums)
    {
        var counts = new HashMap<int, int>();

        foreach (var n in nums)
        {
            counts.TryGetValue(n, out var existing);
            counts.Set(n, existing + 1);
        }

        var result = new List<int>();
        foreach (var key in counts.Keys)
        {
            counts.TryGetValue(key, out var count);
            if (count == 1)
            {
                result.Add(key);
            }
        }

        return result.ToArray();
    }
}
