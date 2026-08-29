using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 3Sum Closest (LC 16): cubic exhaustive scan vs. MergeSort plus a linear
// two-pointer sweep per fixed first element.
[MemoryDiagnoser]
public class ThreeSumClosestBenchmarks
{
    private const int Target = 37;

    private int[] _values = null!;

    [Params(80, 500)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(16);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-Length, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var best = _values[0] + _values[1] + _values[2];

        for (var i = 0; i < _values.Length - 2; i++)
        {
            for (var j = i + 1; j < _values.Length - 1; j++)
            {
                for (var k = j + 1; k < _values.Length; k++)
                {
                    var sum = _values[i] + _values[j] + _values[k];
                    if (Math.Abs(Target - sum) < Math.Abs(Target - best))
                    {
                        best = sum;
                    }
                }
            }
        }

        return best;
    }

    [Benchmark]
    public int MergeSortTwoPointers()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var best = sorted[0] + sorted[1] + sorted[2];
        for (var i = 0; i < sorted.Length - 2; i++)
        {
            var left = i + 1;
            var right = sorted.Length - 1;

            while (left < right)
            {
                var sum = sorted[i] + sorted[left] + sorted[right];
                if (Math.Abs(Target - sum) < Math.Abs(Target - best))
                {
                    best = sum;
                }

                if (sum < Target)
                {
                    left++;
                }
                else if (sum > Target)
                {
                    right--;
                }
                else
                {
                    return Target;
                }
            }
        }

        return best;
    }
}
