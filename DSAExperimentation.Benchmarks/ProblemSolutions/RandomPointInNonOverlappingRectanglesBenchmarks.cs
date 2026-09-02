using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Random Point in Non-overlapping Rectangles (LC 497): a linear weighted scan
// through the cumulative-area prefix sums (walk until the running total exceeds
// the draw, O(n) per Pick) vs. this repo's own BinarySearch.UpperBound over an
// ArraySequence<int> of the same prefix sums (O(log n) per Pick), the same
// prefix-sum-plus-BinarySearch pairing CountOfSmallerNumbersAfterSelfBenchmarks
// already uses for rank lookup. Both draw from the same seeded Random sequence so
// neither benefits from a luckier draw order; the in-rectangle point itself is a
// second, unweighted draw identical in both.
[MemoryDiagnoser]
public class RandomPointInNonOverlappingRectanglesBenchmarks
{
    private const int PickCalls = 500;
    private const int RandomSeed = 497; // LC problem number
    private const int RectangleXStep = 10;
    private const int MaxRectangleDimension = 5;
    private const int RectX2Index = 2;
    private const int RectY2Index = 3;

    [Params(50, 2_000)]
    public int RectangleCount;

    private int[][] _rects = null!;
    private int[] _prefixAreas = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rects = new int[RectangleCount][];
        _prefixAreas = new int[RectangleCount];
        var running = 0;

        for (var i = 0; i < RectangleCount; i++)
        {
            var x1 = i * RectangleXStep;
            var y1 = 0;
            var x2 = x1 + random.Next(1, MaxRectangleDimension);
            var y2 = random.Next(1, MaxRectangleDimension);
            _rects[i] = [x1, y1, x2, y2];
            running += (x2 - x1 + 1) * (y2 - y1 + 1);
            _prefixAreas[i] = running;
        }
    }

    [Benchmark(Baseline = true)]
    public long LinearWeightedScan()
    {
        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var draw = random.Next(_prefixAreas[^1]);
            var index = 0;

            while (_prefixAreas[index] <= draw)
            {
                index++;
            }

            total += PointWithin(_rects[index], random);
        }

        return total;
    }

    [Benchmark]
    public long BinarySearchUpperBound()
    {
        var random = new Random(1);
        var sequence = new ArraySequence<int>(_prefixAreas);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var draw = random.Next(_prefixAreas[^1]);
            var index = BinarySearch.UpperBound(sequence, draw);

            total += PointWithin(_rects[index], random);
        }

        return total;
    }

    private static long PointWithin(int[] rect, Random random)
    {
        var x = rect[0] + random.Next(rect[RectX2Index] - rect[0] + 1);
        var y = rect[1] + random.Next(rect[RectY2Index] - rect[1] + 1);
        return x + y;
    }
}
