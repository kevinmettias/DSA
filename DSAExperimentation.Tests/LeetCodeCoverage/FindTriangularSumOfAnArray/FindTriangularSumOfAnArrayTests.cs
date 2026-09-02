using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTriangularSumOfAnArray;

// LeetCode 2221. Find Triangular Sum of an Array: this repo's own DynamicArray<int>
// backs each round's reduced row - repeatedly replacing every adjacent pair with
// their sum mod 10 (a Pascal's-triangle-style reduction) until exactly one element
// remains. Get/Set/Add/Count are all this problem needs; no closed-form
// modular-binomial-coefficient shortcut is composable from existing primitives (see
// FindTriangularSumOfAnArrayBenchmarks' own doc comment), so the direct simulation
// is what's proven here.
public sealed class FindTriangularSumOfAnArrayTests
{
    [Fact]
    public void TriangularSum_LeetCodeExample1_ReturnsFinalDigit()
    {
        int[] nums = [1, 2, 3, 4, 5];

        var result = TriangularSum(nums);

        Assert.Equal(8, result);
    }

    [Fact]
    public void TriangularSum_LeetCodeExample2_ReturnsThatElement()
    {
        int[] nums = [5];

        var result = TriangularSum(nums);

        Assert.Equal(5, result);
    }

    [Fact]
    public void TriangularSum_TwoElements_ReturnsTheirSumModTen()
    {
        int[] nums = [7, 8];

        var result = TriangularSum(nums);

        Assert.Equal(5, result);
    }

    private static int TriangularSum(int[] nums)
    {
        var current = BuildArray(nums);

        while (current.Count > 1)
        {
            current = ReduceOnce(current);
        }

        return current.Get(0);
    }

    private static DynamicArray<int> BuildArray(int[] nums)
    {
        var array = new DynamicArray<int>();

        foreach (var num in nums)
        {
            array.Add(num);
        }

        return array;
    }

    private static DynamicArray<int> ReduceOnce(DynamicArray<int> array)
    {
        var next = new DynamicArray<int>();

        for (var i = 0; i < array.Count - 1; i++)
        {
            next.Add((array.Get(i) + array.Get(i + 1)) % 10);
        }

        return next;
    }
}
