using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// How Many Numbers Are Smaller Than the Current Number (LC 1365): the textbook
// O(n^2) pairwise-count baseline vs. sort once (this repo's own MergeSort) then
// binary-search each element's insertion point (BinarySearch.LowerBound) -
// O(n log n).
[MemoryDiagnoser]
public class HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarks
{
    private const int RandomSeed = 1365; // LC problem number
    private const int ValueExclusiveBound = 100_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueExclusiveBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce()
    {
        var result = new int[_values.Length];

        for (var i = 0; i < _values.Length; i++)
        {
            var count = 0;

            for (var j = 0; j < _values.Length; j++)
            {
                if (_values[j] < _values[i])
                {
                    count++;
                }
            }

            result[i] = count;
        }

        return result;
    }

    [Benchmark]
    public int[] SortThenBinarySearch()
    {
        var sorted = (int[])_values.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sequence = new ArraySequence<int>(sorted);
        var result = new int[_values.Length];

        for (var i = 0; i < _values.Length; i++)
        {
            result[i] = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, _values[i]);
        }

        return result;
    }
}
