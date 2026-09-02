using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MakeArrayEmpty;

// LeetCode 2659. Make Array Empty: repeatedly remove the front if it's the smallest
// remaining value, else move it to the back - count the operations until empty.
// Reframed as a circular sweep: process original indices in ascending value order
// (the order they actually get removed in) and, for each one, count how many
// still-present slots lie between the previously removed index and this one
// (wrapping around the end) - that count is exactly this step's op total (one
// "move to back" per still-present slot passed over, plus the removal itself).
// FenwickTree<int,SumOperation<int>> (this repo's own Binary Indexed Tree) tracks
// "still present" as a 0/1 point-updatable array and answers each range's present
// count in O(log n), the same coordinate-free range-count shape
// CountGoodTripletsInAnArrayTests already uses for LC 2179.
public sealed class MakeArrayEmptyTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [3, 4, -1], 5 },
            { [1, 2, 4, 3], 5 },
            { [1, 2, 3], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountOperationsByLinearScan_LeetCodeExamples_ReturnsExpectedOperationCount(int[] nums, long expected)
    {
        var actual = CountOperationsByLinearScan(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountOperationsByFenwickTree_LeetCodeExamples_ReturnsExpectedOperationCount(int[] nums, long expected)
    {
        var actual = CountOperationsByFenwickTree(nums);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CountOperationsByFenwickTree_SingleElement_ReturnsOne()
    {
        var actual = CountOperationsByFenwickTree([42]);

        Assert.Equal(1L, actual);
    }

    // O(n) present-count scan per step instead of FenwickTree's O(log n) range query -
    // otherwise the exact same circular-sweep algorithm, so it doubles as a
    // correctness cross-check for the FenwickTree-backed version.
    private static long CountOperationsByLinearScan(int[] nums)
    {
        var n = nums.Length;
        var order = Enumerable.Range(0, n).OrderBy(i => nums[i]).ToArray();
        var present = new bool[n];
        Array.Fill(present, true);

        long total = 0;
        var prev = -1;

        foreach (var idx in order)
        {
            total += idx > prev
                ? CountPresent(present, prev + 1, idx)
                : CountPresent(present, prev + 1, n - 1) + CountPresent(present, 0, idx);

            present[idx] = false;
            prev = idx;
        }

        return total;
    }

    private static int CountPresent(bool[] present, int left, int right)
    {
        var count = 0;
        for (var i = left; i <= right; i++)
        {
            if (present[i])
            {
                count++;
            }
        }

        return count;
    }

    private static long CountOperationsByFenwickTree(int[] nums)
    {
        var n = nums.Length;
        var order = Enumerable.Range(0, n).OrderBy(i => nums[i]).ToArray();
        var present = new int[n];
        Array.Fill(present, 1);
        var tree = new FenwickTree<int, SumOperation<int>>(present);

        long total = 0;
        var prev = -1;

        foreach (var idx in order)
        {
            total += idx > prev
                ? PresentCount(tree, prev + 1, idx)
                : PresentCount(tree, prev + 1, n - 1) + PresentCount(tree, 0, idx);

            tree.Add(idx, -1);
            prev = idx;
        }

        return total;
    }

    private static int PresentCount(FenwickTree<int, SumOperation<int>> tree, int left, int right)
        => left > right ? 0 : tree.Query(left, right);
}
