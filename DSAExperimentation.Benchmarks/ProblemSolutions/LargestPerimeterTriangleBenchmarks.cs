using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Largest Perimeter Triangle (LC 976): the cubic every-triple scan for the best
// valid triangle vs. this repo's own MergeSort.Sort<int,ArrayIndexedSequence<int>>
// (ThreeSumBenchmarks precedent) followed by one backward scan that only ever
// checks consecutive triples off the sorted top.
[MemoryDiagnoser]
public class LargestPerimeterTriangleBenchmarks
{
    [Params(80, 300)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(976);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var best = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                for (var k = j + 1; k < _values.Length; k++)
                {
                    int a = _values[i], b = _values[j], c = _values[k];

                    if (a + b > c && a + c > b && b + c > a)
                    {
                        best = Math.Max(best, a + b + c);
                    }
                }
            }
        }

        return best;
    }

    [Benchmark]
    public int SortThenScan()
    {
        var sorted = (int[])_values.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        for (var i = sorted.Length - 1; i >= 2; i--)
        {
            if (sorted[i - 2] + sorted[i - 1] > sorted[i])
            {
                return sorted[i - 2] + sorted[i - 1] + sorted[i];
            }
        }

        return 0;
    }
}
