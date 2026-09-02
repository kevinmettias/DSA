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
    private const int RandomSeed = 976; // LC problem number
    private const int SideValueUpperBound = 1_000; // exclusive upper bound for generated side lengths
    private const int TripleWindowOffset = 2; // sorted[i-TripleWindowOffset..i] form the candidate triple

    [Params(80, 300)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, SideValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var best = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                best = BestPerimeterWithFixedPair(i, j, best);
            }
        }

        return best;
    }

    private int BestPerimeterWithFixedPair(int i, int j, int best)
    {
        for (var k = j + 1; k < _values.Length; k++)
        {
            int a = _values[i], b = _values[j], c = _values[k];

            if (a + b > c && a + c > b && b + c > a)
            {
                best = Math.Max(best, a + b + c);
            }
        }

        return best;
    }

    [Benchmark]
    public int SortThenScan()
    {
        var sorted = (int[])_values.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        for (var i = sorted.Length - 1; i >= TripleWindowOffset; i--)
        {
            if (sorted[i - TripleWindowOffset] + sorted[i - 1] > sorted[i])
            {
                return sorted[i - TripleWindowOffset] + sorted[i - 1] + sorted[i];
            }
        }

        return 0;
    }
}
