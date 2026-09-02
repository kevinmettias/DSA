using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfEvenNumbersAfterQueries;

// LeetCode 985. Sum of Even Numbers After Queries: this repo's own DynamicArray<int>
// (O(1) Get/Set, the same "array problem via this repo's own array type"
// composition ArrayPartitionTests already uses) backs nums; a running even-sum
// invariant is updated in O(1) per query - only the touched index's even/odd
// status can change - instead of rescanning the whole array after every update
// (naively O(n) per query, O(n*q) overall).
public sealed partial class SumOfEvenNumbersAfterQueriesTests
{
    [Fact]
    public void SumEvenAfterQueries_LeetCodeExample_ReturnsRunningEvenSums()
    {
        int[] nums = [1, 2, 3, 4];
        int[][] queries = [[1, 0], [-3, 1], [-4, 0], [2, 3]];

        var result = SumEvenAfterQueries(nums, queries);

        Assert.Equal([8, 6, 2, 4], result);
    }

    [Fact]
    public void SumEvenAfterQueries_SingleOddElementStaysOdd_ReturnsZero()
    {
        int[] nums = [1];
        int[][] queries = [[4, 0]];

        var result = SumEvenAfterQueries(nums, queries);

        Assert.Equal([0], result);
    }

    private static int[] SumEvenAfterQueries(int[] nums, int[][] queries)
    {
        var array = BuildArray(nums);
        var evenSum = ComputeInitialEvenSum(array);
        var results = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            evenSum = ApplyQuery(array, queries[q], evenSum);
            results[q] = evenSum;
        }

        return results;
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

    private static int ComputeInitialEvenSum(DynamicArray<int> array)
    {
        var evenSum = 0;

        for (var i = 0; i < array.Count; i++)
        {
            if (array.Get(i) % 2 == 0)
            {
                evenSum += array.Get(i);
            }
        }

        return evenSum;
    }

    private static int ApplyQuery(DynamicArray<int> array, int[] query, int evenSum)
    {
        var value = query[0];
        var index = query[1];
        var before = array.Get(index);

        if (before % 2 == 0)
        {
            evenSum -= before;
        }

        var after = before + value;
        array.Set(index, after);

        if (after % 2 == 0)
        {
            evenSum += after;
        }

        return evenSum;
    }
}
