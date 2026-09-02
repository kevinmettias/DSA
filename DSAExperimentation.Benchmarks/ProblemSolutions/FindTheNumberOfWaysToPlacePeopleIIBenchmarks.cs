using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheNumberOfWaysToPlacePeopleIISolution's,
// the same methods FindTheNumberOfWaysToPlacePeopleIITests proves correct.
// Coordinate range matches LC 3027's own bound (-10^9 <= coordinate <= 10^9);
// Length tops out at 500 rather than LC 3027's full n <= 1000, because
// BruteForce is cubic (unlike MaximumStrongPairXORII's quadratic baseline,
// which can afford its own full bound) - 500 is already well past where
// SortedSweep pulls ahead. SortedSweep is handed the pre-sorted
// ArrayIndexedSequence<int[]> its hoisted overload takes, sorted by the
// solution's own ByXThenDescendingY rule, so sorting is charged to
// [GlobalSetup] rather than to the sweep being measured.
[MemoryDiagnoser]
public class FindTheNumberOfWaysToPlacePeopleIIBenchmarks
{
    private const int RandomSeed = 3027;
    private const int MaxCoordinate = 1_000_000_000;

    [Params(10, 100, 500)]
    public int Length;

    private int[][] _points = null!;
    private ArrayIndexedSequence<int[]> _sortedPoints;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Every x-coordinate is a distinct value from a shuffled 0..Length-1
        // range, offset into LC 3027's coordinate range, which alone
        // guarantees every point is distinct, as the problem requires.
        _points = Enumerable.Range(0, Length)
            .OrderBy(_ => random.Next())
            .Select(x => new[] { x - Length / 2, random.Next(-MaxCoordinate, MaxCoordinate) })
            .ToArray();

        _sortedPoints = new ArrayIndexedSequence<int[]>((int[][])_points.Clone());
        MergeSort.Sort<int[], ArrayIndexedSequence<int[]>>(
            _sortedPoints, FindTheNumberOfWaysToPlacePeopleIISolution.ByXThenDescendingY);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FindTheNumberOfWaysToPlacePeopleIISolution.CountPairsByBruteForce(_points);

    [Benchmark]
    public int SortedSweep() => FindTheNumberOfWaysToPlacePeopleIISolution.CountPairsBySortedSweep(_sortedPoints);
}
