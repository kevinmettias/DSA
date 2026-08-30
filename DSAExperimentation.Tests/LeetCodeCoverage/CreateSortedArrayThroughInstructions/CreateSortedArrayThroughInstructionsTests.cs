using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CreateSortedArrayThroughInstructions;

// LeetCode 1649. Create Sorted Array through Instructions: the same value-indexed
// FenwickTree<int, SumOperation<int>> sweep CountOfSmallerNumbersAfterSelfTests
// uses for LeetCode 315, run left to right instead of right to left. PrefixQuery
// (value-1) counts every strictly-smaller element already inserted; the running
// insertion count minus PrefixQuery(value) counts every strictly-greater one -
// each instruction's cost is the smaller of the two.
public sealed partial class CreateSortedArrayThroughInstructionsTests
{
    [Fact]
    public void CreateSortedArray_ClassicExample_ReturnsMinimumTotalCost()
        => Assert.Equal(1, CreateSortedArray([1, 5, 6, 2]));

    [Fact]
    public void CreateSortedArray_StrictlyIncreasing_CostsNothing()
        => Assert.Equal(0, CreateSortedArray([1, 2, 3, 4, 5]));

    [Fact]
    public void CreateSortedArray_RiseThenFall_SumsCostOfEachOutOfOrderInsertion()
        => Assert.Equal(3, CreateSortedArray([1, 2, 3, 6, 5, 4]));

    [Fact]
    public void CreateSortedArray_DuplicateValues_SumsMinimumOfLessAndGreaterCounts()
        => Assert.Equal(4, CreateSortedArray([1, 3, 3, 3, 2, 4, 2, 1, 2]));

    private static int CreateSortedArray(int[] instructions)
    {
        const int Modulus = 1_000_000_007;

        var maxValue = instructions.Max();
        var tree = new FenwickTree<int, SumOperation<int>>(maxValue + 1);
        long cost = 0;

        for (var i = 0; i < instructions.Length; i++)
        {
            var value = instructions[i];
            var lessCount = value == 0 ? 0 : tree.PrefixQuery(value - 1);
            var greaterCount = i - tree.PrefixQuery(value);
            cost += Math.Min(lessCount, greaterCount);
            tree.Add(value, 1);
        }

        return (int)(cost % Modulus);
    }
}
