using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 3Sum With Multiplicity (LC 923): cubic triple-loop counting (baseline, the
// textbook approach) vs. MergeSort plus the sorted two-pointer sweep that counts
// each equal-valued span's combinations directly instead of visiting one pair at
// a time - the same MergeSort/ArrayIndexedSequence primitives ThreeSumBenchmarks
// already uses, extended with multiplicity counting under a modulus.
[MemoryDiagnoser]
public class ThreeSumWithMultiplicityBenchmarks
{
    private const int Modulo = 1_000_000_007;
    private const int Target = 150;

    [Params(80, 300)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(923);
        // Values bounded to [0, 100], matching LC 923's own constraint - this small
        // range is what forces genuine multiplicities (repeated values), which is
        // the whole point of this problem versus plain 3Sum.
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, 101)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        long count = 0;

        for (var i = 0; i < _values.Length - 2; i++)
        {
            for (var j = i + 1; j < _values.Length - 1; j++)
            {
                for (var k = j + 1; k < _values.Length; k++)
                {
                    if (_values[i] + _values[j] + _values[k] == Target)
                    {
                        count++;
                    }
                }
            }
        }

        return (int)(count % Modulo);
    }

    [Benchmark]
    public int MergeSortTwoPointers()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        long count = 0;

        for (var i = 0; i < sorted.Length - 2; i++)
        {
            var remaining = Target - sorted[i];
            var left = i + 1;
            var right = sorted.Length - 1;

            while (left < right)
            {
                var pairSum = sorted[left] + sorted[right];

                if (pairSum < remaining)
                {
                    left++;
                }
                else if (pairSum > remaining)
                {
                    right--;
                }
                else if (sorted[left] != sorted[right])
                {
                    var leftCount = 1;
                    while (left + 1 < right && sorted[left + 1] == sorted[left])
                    {
                        leftCount++;
                        left++;
                    }

                    var rightCount = 1;
                    while (right - 1 > left && sorted[right - 1] == sorted[right])
                    {
                        rightCount++;
                        right--;
                    }

                    count = (count + ((long)leftCount * rightCount)) % Modulo;
                    left++;
                    right--;
                }
                else
                {
                    var span = right - left + 1;
                    count = (count + ((long)span * (span - 1) / 2)) % Modulo;
                    break;
                }
            }
        }

        return (int)count;
    }
}
