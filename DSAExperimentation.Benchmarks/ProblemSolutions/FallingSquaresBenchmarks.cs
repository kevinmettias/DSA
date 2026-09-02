using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Falling Squares (LC 699): the classic O(n^2) "check every earlier square's footprint for
// overlap" brute force vs. the O(n log n) coordinate-compression + this repo's own
// LazySegmentTree<int,int?,RangeAssignMaxOperation<int>> approach (range-max query for the
// height already stacked under a square's footprint, range-assign to plant the new height
// across it). Footprints are randomly overlapping so both strategies pay their full
// worst-case cost rather than degenerating to disjoint, non-interacting squares.
[MemoryDiagnoser]
public class FallingSquaresBenchmarks
{
    private const int PositionRangeMultiplier = 2;
    private const int MaxSquareSize = 50;
    private const int CoordinatesPerPosition = 2;

    [Params(100, 1_000)]
    public int SquareCount;

    private int[][] _positions = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _positions = new int[SquareCount][];

        for (var i = 0; i < SquareCount; i++)
        {
            var left = random.Next(0, SquareCount * PositionRangeMultiplier);
            var size = random.Next(1, MaxSquareSize);
            _positions[i] = [left, size];
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> BruteForceOverlapScan()
    {
        var heights = new int[_positions.Length];
        var result = new List<int>(_positions.Length);
        var overallMax = 0;

        for (var i = 0; i < _positions.Length; i++)
        {
            overallMax = StackSquareAndTrackMax(i, heights, overallMax);
            result.Add(overallMax);
        }

        return result;
    }

    private int StackSquareAndTrackMax(int i, int[] heights, int overallMax)
    {
        var left = _positions[i][0];
        var right = left + _positions[i][1];
        var heightBelow = 0;

        for (var j = 0; j < i; j++)
        {
            var otherLeft = _positions[j][0];
            var otherRight = otherLeft + _positions[j][1];

            if (left < otherRight && otherLeft < right && heights[j] > heightBelow)
            {
                heightBelow = heights[j];
            }
        }

        heights[i] = heightBelow + _positions[i][1];
        return Math.Max(overallMax, heights[i]);
    }

    [Benchmark]
    public List<int> LazySegmentTreeRangeMax()
    {
        var coordinates = CompressCoordinates(_positions);
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(new int[coordinates.Length - 1]);

        var result = new List<int>(_positions.Length);
        var overallMax = 0;

        foreach (var position in _positions)
        {
            var left = Array.BinarySearch(coordinates, position[0]);
            var right = Array.BinarySearch(coordinates, position[0] + position[1]) - 1;

            var heightBelow = tree.Query(left, right);
            var newHeight = heightBelow + position[1];

            tree.UpdateRange(left, right, newHeight);
            overallMax = Math.Max(overallMax, newHeight);
            result.Add(overallMax);
        }

        return result;
    }

    private static int[] CompressCoordinates(int[][] positions)
    {
        var coordinates = new List<int>(positions.Length * CoordinatesPerPosition);

        foreach (var position in positions)
        {
            coordinates.Add(position[0]);
            coordinates.Add(position[0] + position[1]);
        }

        return coordinates.Distinct().OrderBy(x => x).ToArray();
    }
}
