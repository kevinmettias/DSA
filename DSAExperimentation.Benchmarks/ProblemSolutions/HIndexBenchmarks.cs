using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// H-Index (LC 274): the textbook O(n^2) "recount papers >= h for every candidate h"
// scan vs. this repo's own O(n log n) MergeSort over ArrayIndexedSequence followed by
// a single O(n) scan from the most-cited paper down.
[MemoryDiagnoser]
public class HIndexBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _citations = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(274);
        _citations = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int CountForEveryCandidate()
    {
        var h = 0;

        for (var candidate = 0; candidate <= _citations.Length; candidate++)
        {
            var count = 0;
            foreach (var citation in _citations)
            {
                if (citation >= candidate)
                {
                    count++;
                }
            }

            if (count >= candidate)
            {
                h = candidate;
            }
        }

        return h;
    }

    [Benchmark]
    public int MergeSortScan()
    {
        var sorted = _citations.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var h = 0;
        for (var i = sorted.Length - 1; i >= 0; i--)
        {
            var papersAtLeastThisCited = sorted.Length - i;
            if (sorted[i] < papersAtLeastThisCited)
            {
                break;
            }

            h = papersAtLeastThisCited;
        }

        return h;
    }
}
