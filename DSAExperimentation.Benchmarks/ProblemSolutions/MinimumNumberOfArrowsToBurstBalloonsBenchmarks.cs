using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Arrows to Burst Balloons (LC 452): the O(n^2) brute force
// (repeatedly rescan every unburst balloon for the minimum end, then burst
// everything it reaches) vs. this repo's own O(n log n) MergeSort over
// ArrayIndexedSequence to sort once by end, followed by a single O(n) greedy
// pass (MinimumNumberOfArrowsToBurstBalloonsTests' algorithm). _points is
// generated as mostly non-overlapping, shuffled balloons (needing close to n
// arrows), which hits brute force's true worst case - its outer "find the next
// arrow" loop runs close to n times, each paying a full O(n) rescan - instead
// of the few-arrows case where heavy overlap lets it finish in a handful of
// passes and look artificially competitive.
[MemoryDiagnoser]
public class MinimumNumberOfArrowsToBurstBalloonsBenchmarks
{
    [Params(200, 3_000)]
    public int Length;

    private (int Start, int End)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(452);
        _points = Enumerable.Range(0, Length)
            .Select(i => (Start: i * 3, End: i * 3 + random.Next(0, 2)))
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRepeatedMinEndScan()
    {
        var burst = new bool[_points.Length];
        var remaining = _points.Length;
        var arrows = 0;

        while (remaining > 0)
        {
            var minEnd = int.MaxValue;

            for (var i = 0; i < _points.Length; i++)
            {
                if (!burst[i] && _points[i].End < minEnd)
                {
                    minEnd = _points[i].End;
                }
            }

            arrows++;

            for (var i = 0; i < _points.Length; i++)
            {
                if (!burst[i] && _points[i].Start <= minEnd && minEnd <= _points[i].End)
                {
                    burst[i] = true;
                    remaining--;
                }
            }
        }

        return arrows;
    }

    [Benchmark]
    public int SortByEndThenGreedyScan()
    {
        var items = ((int Start, int End)[])_points.Clone();

        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(items),
            Comparer<(int Start, int End)>.Create((a, b) => a.End.CompareTo(b.End)));

        var arrows = 1;
        var arrowPosition = items[0].End;

        for (var i = 1; i < items.Length; i++)
        {
            if (items[i].Start > arrowPosition)
            {
                arrows++;
                arrowPosition = items[i].End;
            }
        }

        return arrows;
    }
}
