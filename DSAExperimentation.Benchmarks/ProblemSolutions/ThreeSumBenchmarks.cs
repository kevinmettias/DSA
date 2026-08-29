using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 3Sum (LC 15): cubic duplicate-filtered brute force vs. MergeSort plus the
// sorted two-pointer sweep.
[MemoryDiagnoser]
public class ThreeSumBenchmarks
{
    private int[] _values = null!;

    [Params(80, 300)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(15);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-Length, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var found = new HashSet<(int First, int Second, int Third)>();

        for (var i = 0; i < _values.Length - 2; i++)
        {
            for (var j = i + 1; j < _values.Length - 1; j++)
            {
                for (var k = j + 1; k < _values.Length; k++)
                {
                    if (_values[i] + _values[j] + _values[k] == 0)
                    {
                        int[] triplet = [_values[i], _values[j], _values[k]];
                        Array.Sort(triplet);
                        found.Add((triplet[0], triplet[1], triplet[2]));
                    }
                }
            }
        }

        return found.Count;
    }

    [Benchmark]
    public int MergeSortTwoPointers()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var count = 0;
        for (var i = 0; i < sorted.Length - 2; i++)
        {
            if (i > 0 && sorted[i] == sorted[i - 1])
            {
                continue;
            }

            var left = i + 1;
            var right = sorted.Length - 1;

            while (left < right)
            {
                var sum = sorted[i] + sorted[left] + sorted[right];
                if (sum == 0)
                {
                    count++;
                    left++;
                    right--;

                    while (left < right && sorted[left] == sorted[left - 1])
                    {
                        left++;
                    }

                    while (left < right && sorted[right] == sorted[right + 1])
                    {
                        right--;
                    }
                }
                else if (sum < 0)
                {
                    left++;
                }
                else
                {
                    right--;
                }
            }
        }

        return count;
    }
}
