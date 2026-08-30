using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NRepeatedElementInSize2NArray;

// LeetCode 961. N-Repeated Element in Size 2N Array: a single pass with this
// repo's own Set<int> - the first value TryAdd refuses (because it's already
// present) is the one repeated N times. Same "have I seen this before" role
// NumberOfProvincesTests already uses Set<int> for when counting distinct roots,
// just checked on every element instead of only at the end.
public sealed partial class NRepeatedElementInSize2NArrayTests
{
    [Fact]
    public void FindRepeatedElement_ClassicExample_ReturnsTheRepeatedValue()
    {
        int[] nums = [1, 2, 3, 3];

        Assert.Equal(3, FindRepeatedElement(nums));
    }

    [Fact]
    public void FindRepeatedElement_RepeatedValueAppearsLast_StillFindsIt()
    {
        int[] nums = [5, 1, 5, 2, 5, 3, 5, 4];

        Assert.Equal(5, FindRepeatedElement(nums));
    }

    [Fact]
    public void FindRepeatedElement_SmallestValidInput_ReturnsTheRepeatedValue()
    {
        // n = 2: 4 elements, n + 1 = 3 unique values, one repeated exactly n times.
        int[] nums = [4, 5, 4, 6];

        Assert.Equal(4, FindRepeatedElement(nums));
    }

    private static int FindRepeatedElement(int[] nums)
    {
        var seen = new Set<int>();

        foreach (var value in nums)
        {
            if (!seen.TryAdd(value))
            {
                return value;
            }
        }

        throw new InvalidOperationException("No repeated element found - input violates the problem's own precondition.");
    }
}
