using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheNumberOfWaysToPlacePeopleISolution's,
// the same methods FindTheNumberOfWaysToPlacePeopleITests proves correct.
// Length and coordinate range match LC 3025's own bound (n <= 50, 0 <=
// coordinate <= 50), where the O(n^3) scan is already fast - see
// FindTheNumberOfWaysToPlacePeopleIIBenchmarks for the same comparison at the
// scale that actually favors the sweep strategy. SortedSweep is handed the
// pre-sorted ArrayIndexedSequence<int[]> its hoisted overload takes, sorted
// by PointOrder.ByXThenDescendingY, the rule the sweep is defined over, so
// sorting is charged to [GlobalSetup] rather than to the sweep being measured.
[MemoryDiagnoser]
public class FindTheNumberOfWaysToPlacePeopleIBenchmarks
{
    private const int RandomSeed = 3025;
    private const int MaxCoordinateExclusive = 51;

    private int[][] _points = [];

    private ArrayIndexedSequence<int[]> _sortedPoints;
    [Params(10, 50)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Every x-coordinate is a distinct value from a shuffled 0..Length-1
        // range, which alone guarantees every point is distinct, as LC 3025
        // requires.
        _points = Enumerable.Range(0, Length)
            .OrderBy(_ => random.Next())
            .Select(x => new[] { x, random.Next(0, MaxCoordinateExclusive) })
            .ToArray();

        _sortedPoints = new ArrayIndexedSequence<int[]>((int[][])_points.Clone());
        MergeSort.Sort<int[], ArrayIndexedSequence<int[]>>(
            _sortedPoints, PointOrder.ByXThenDescendingY);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FindTheNumberOfWaysToPlacePeopleISolution.CountPairsByBruteForce(_points);

    [Benchmark]
    public int SortedSweep() => FindTheNumberOfWaysToPlacePeopleISolution.CountPairsBySortedSweep(_sortedPoints);
}
