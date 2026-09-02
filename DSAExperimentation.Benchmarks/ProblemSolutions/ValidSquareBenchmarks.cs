using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Square (LC 593): checking a single quad is inherently O(1) work (6 pairwise
// distances), so both strategies below run across a large batch of quads to make
// the comparison measurable - a manual two-pass min/max scan over the 6 squared
// distances vs. this repo's own MergeSort over ArrayIndexedSequence (the same
// "sort, then read the shape off the sorted result" idiom HIndexBenchmarks uses)
// followed by a fixed-index check. Half the batch is real squares (random center/
// half-side, axis-aligned), half is four independently-random points, so neither
// strategy gets to shortcut on an all-true or all-false batch.
[MemoryDiagnoser]
public class ValidSquareBenchmarks
{
    private const int RandomSeed = 593; // LC problem number
    private const int AlternationModulus = 2;
    private const int CoordinateBound = 1_000;
    private const int MaxHalfSide = 500;
    private const int ExpectedSideCount = 4;
    private const int ExpectedDiagonalCount = 2;
    private const int DiagonalToSideSquaredRatio = 2;
    private const int ThirdSideIndex = 2;
    private const int FourthSideIndex = 3;
    private const int FirstDiagonalIndex = 4;
    private const int SecondDiagonalIndex = 5;
    private const int PairwiseDistanceCount = 6;

    [Params(5_000, 100_000)]
    public int BatchCount;

    private int[][][] _batches = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _batches = new int[BatchCount][][];

        for (var i = 0; i < BatchCount; i++)
        {
            _batches[i] = i % AlternationModulus == 0 ? RandomSquare(random) : RandomQuad(random);
        }
    }

    private static int[][] RandomSquare(Random random)
    {
        var cx = random.Next(-CoordinateBound, CoordinateBound);
        var cy = random.Next(-CoordinateBound, CoordinateBound);
        var half = random.Next(1, MaxHalfSide);

        return
        [
            [cx - half, cy - half],
            [cx - half, cy + half],
            [cx + half, cy + half],
            [cx + half, cy - half],
        ];
    }

    private static int[][] RandomQuad(Random random) =>
    [
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
    ];

    [Benchmark(Baseline = true)]
    public int ManualMinMaxScan()
    {
        var validCount = 0;

        foreach (var points in _batches)
        {
            if (IsValidSquareManualScan(points))
            {
                validCount++;
            }
        }

        return validCount;
    }

    [Benchmark]
    public int MergeSortThenScan()
    {
        var validCount = 0;

        foreach (var points in _batches)
        {
            if (IsValidSquareMergeSort(points))
            {
                validCount++;
            }
        }

        return validCount;
    }

    private static bool IsValidSquareManualScan(int[][] points)
    {
        var distances = SquaredDistances(points);
        var (min, max) = FindMinAndMax(distances);

        if (min <= 0)
        {
            return false;
        }

        return HasValidSideAndDiagonalCounts(distances, min, max);
    }

    private static (long Min, long Max) FindMinAndMax(long[] distances)
    {
        var min = long.MaxValue;
        var max = long.MinValue;
        foreach (var distance in distances)
        {
            min = Math.Min(min, distance);
            max = Math.Max(max, distance);
        }

        return (min, max);
    }

    private static bool HasValidSideAndDiagonalCounts(long[] distances, long min, long max)
    {
        var sideCount = 0;
        var diagonalCount = 0;
        foreach (var distance in distances)
        {
            if (distance == min)
            {
                sideCount++;
            }
            else if (distance == max)
            {
                diagonalCount++;
            }
            else
            {
                return false;
            }
        }

        return sideCount == ExpectedSideCount && diagonalCount == ExpectedDiagonalCount && max == DiagonalToSideSquaredRatio * min;
    }

    private static bool IsValidSquareMergeSort(int[][] points)
    {
        var distances = SquaredDistances(points);
        MergeSort.Sort<long, ArrayIndexedSequence<long>>(new ArrayIndexedSequence<long>(distances));

        var side = distances[0];
        return side > 0
            && distances[1] == side && distances[ThirdSideIndex] == side && distances[FourthSideIndex] == side
            && distances[FirstDiagonalIndex] == distances[SecondDiagonalIndex] && distances[FirstDiagonalIndex] == DiagonalToSideSquaredRatio * side;
    }

    private static long[] SquaredDistances(int[][] points)
    {
        var distances = new long[PairwiseDistanceCount];
        var next = 0;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var dx = points[i][0] - points[j][0];
                var dy = points[i][1] - points[j][1];
                distances[next++] = ((long)dx * dx) + ((long)dy * dy);
            }
        }

        return distances;
    }
}
