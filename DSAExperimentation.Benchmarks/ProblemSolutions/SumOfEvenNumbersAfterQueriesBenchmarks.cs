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
    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(985);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-1_000, 1_000)).ToArray();
        _queries = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(-1_000, 1_000), random.Next(0, Length) })
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
                if (nums[i] % 2 == 0)
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
        var array = new DynamicArray<int>();

        foreach (var num in _nums)
        {
            array.Add(num);
        }

        var evenSum = 0;

        for (var i = 0; i < array.Count; i++)
        {
            if (array.Get(i) % 2 == 0)
            {
                evenSum += array.Get(i);
            }
        }

        var results = new int[_queries.Length];

        for (var q = 0; q < _queries.Length; q++)
        {
            var value = _queries[q][0];
            var index = _queries[q][1];
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

            results[q] = evenSum;
        }

        return results;
    }
}
