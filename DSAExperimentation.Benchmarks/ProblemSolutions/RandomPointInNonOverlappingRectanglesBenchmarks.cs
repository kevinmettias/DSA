using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RandomPointInNonOverlappingRectangles;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomPointInNonOverlappingRectanglesSolution's, the
// same methods RandomPointInNonOverlappingRectanglesTests proves correct. Each arm
// is handed the prepared prefix-sum array its hoisted overload takes, so building
// it is charged to [GlobalSetup] rather than to the picks being measured. Both
// draw from a fresh, identically-seeded Random per invocation so neither benefits
// from a luckier draw order; the summed coordinates avoid materializing an int[]
// per pick while still exercising the real point both arms return.
[MemoryDiagnoser]
public class RandomPointInNonOverlappingRectanglesBenchmarks
{
    private const int PickCalls = 500;
    private const int RandomSeed = 497; // LC problem number
    private const int RectangleXStep = 10;
    private const int MaxRectangleDimension = 5;

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
    public long LinearScan()
    {
        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var point =
                RandomPointInNonOverlappingRectanglesSolution.PickByLinearScan(_rects, _prefixAreas, random);
            total += point[0] + point[1];
        }

        return total;
    }

    [Benchmark]
    public long BinarySearchUpperBound()
    {
        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var point = RandomPointInNonOverlappingRectanglesSolution.PickByBinarySearchUpperBound(
                _rects, _prefixAreas, random);
            total += point[0] + point[1];
        }

        return total;
    }
}
