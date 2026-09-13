using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.SumOfEvenNumbersAfterQueries;

// LeetCode 985. Sum of Even Numbers After Queries: each query (value, index) adds
// value to nums[index] and then reports the sum of the array's even entries, so
// the answer is the sequence of those running sums.
//
// SumEvenAfterQueriesByRescan re-adds the even entries from scratch after every
// query: O(n) per query, O(n * q) overall. SumEvenAfterQueriesByRunningEvenSum
// keeps the even sum as an invariant over this repo's own DynamicArray<int> (the
// same "array problem via this repo's own array type" composition
// ArrayPartitionSolution uses) and repairs it in O(1) per query - only the touched
// index's even/odd status can change, so subtracting its old value when it was
// even and adding its new value when it is even is the whole update.
internal static class SumOfEvenNumbersAfterQueriesSolution
{
    private const int EvenDivisor = 2;

    // The textbook answer: mutate a plain int[] and total its even entries again
    // after every query. Deliberately written without this repo's primitives - it
    // is the arm the composed strategy below has to justify itself against.
    public static int[] SumEvenAfterQueriesByRescan(int[] nums, int[][] queries)
    {
        var values = (int[])nums.Clone();
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            values[queries[q][1]] += queries[q][0];
            answers[q] = SumOfEvenValues(values);
        }

        return answers;
    }

    private static int SumOfEvenValues(int[] values)
    {
        var evenSum = 0;

        for (var i = 0; i < values.Length; i++)
        {
            if (values[i] % EvenDivisor == 0)
            {
                evenSum += values[i];
            }
        }

        return evenSum;
    }

    public static int[] SumEvenAfterQueriesByRunningEvenSum(int[] nums, int[][] queries)
    {
        var array = PopulateArray(nums);
        var evenSum = InitialEvenSum(array);
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            evenSum = ApplyQuery(array, queries[q], evenSum);
            answers[q] = evenSum;
        }

        return answers;
    }

    private static DynamicArray<int> PopulateArray(int[] nums)
    {
        var array = new DynamicArray<int>();

        foreach (var num in nums)
        {
            array.Add(num);
        }

        return array;
    }

    private static int InitialEvenSum(DynamicArray<int> array)
    {
        var evenSum = 0;

        for (var i = 0; i < array.Count; i++)
        {
            if (array.Get(i) % EvenDivisor == 0)
            {
                evenSum += array.Get(i);
            }
        }

        return evenSum;
    }

    // The touched index is the only entry whose parity can change, so the invariant
    // is repaired by removing its old contribution and adding its new one.
    private static int ApplyQuery(DynamicArray<int> array, int[] query, int evenSum)
    {
        var value = query[0];
        var index = query[1];
        var before = array.Get(index);

        if (before % EvenDivisor == 0)
        {
            evenSum -= before;
        }

        var after = before + value;
        array.Set(index, after);

        if (after % EvenDivisor == 0)
        {
            evenSum += after;
        }

        return evenSum;
    }
}
