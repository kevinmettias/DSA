using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of Even Numbers After Queries (LC 985): rescanning nums after every query
// (naive O(n*q)) vs. maintaining a running even-sum invariant, updated in O(1) per
// query, over this repo's own DynamicArray<int> (ArrayPartitionTests precedent for
// "array problem via this repo's own array type" - here it's the mutable backing
// store the running-sum trick updates in place).
[MemoryDiagnoser]
public class SumOfEvenNumbersAfterQueriesBenchmarks
{
    private const int RandomSeed = 985; // LC problem number
    private const int ValueMagnitudeBound = 1_000;
    private const int EvenDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitudeBound, ValueMagnitudeBound)).ToArray();
        _queries = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(-ValueMagnitudeBound, ValueMagnitudeBound), random.Next(0, Length) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] RescanAfterEveryQuery()
    {
        var nums = (int[])_nums.Clone();
        var results = new int[_queries.Length];

        for (var q = 0; q < _queries.Length; q++)
        {
            nums[_queries[q][1]] += _queries[q][0];

            var sum = 0;

            for (var i = 0; i < nums.Length; i++)
            {
                if (nums[i] % EvenDivisor == 0)
                {
                    sum += nums[i];
                }
            }

            results[q] = sum;
        }

        return results;
    }

    [Benchmark]
    public int[] RunningEvenSum()
    {
        var array = PopulateArray(_nums);
        var evenSum = ComputeInitialEvenSum(array);
        return ApplyQueriesTrackingEvenSum(array, evenSum);
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

    private static int ComputeInitialEvenSum(DynamicArray<int> array)
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

    private int[] ApplyQueriesTrackingEvenSum(DynamicArray<int> array, int evenSum)
    {
        var results = new int[_queries.Length];

        for (var q = 0; q < _queries.Length; q++)
        {
            evenSum = ApplyQueryToEvenSum(array, _queries[q], evenSum);
            results[q] = evenSum;
        }

        return results;
    }

    private static int ApplyQueryToEvenSum(DynamicArray<int> array, int[] query, int evenSum)
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
