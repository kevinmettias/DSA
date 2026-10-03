using DSAExperimentation.LeetCode.RandomPointInNonOverlappingRectangles;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomPointInNonOverlappingRectanglesSolution's, the
// same methods RandomPointInNonOverlappingRectanglesSolutionTests proves correct. Each arm
// is handed the prepared prefix-sum array its hoisted overload takes, so building
// it is charged to [GlobalSetup] rather than to the picks being measured. Both
// draw from a fresh, identically-seeded Random per invocation so neither benefits
// from a luckier draw order; each returns every point it picked, in call order.
public class RandomPointInNonOverlappingRectanglesBenchmarks
{
    private const int PickCalls = 500;
    private const int RandomSeed = 497; // LC problem number
    private const int RectangleXStep = 10;
    private const int MaxRectangleDimension = 5;

    private int[][] _rects = [];

    private int[] _prefixAreas = [];

    private int[][] _points = [];
    [Params(50, 2_000)]
    public int RectangleCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rects = new int[RectangleCount][];
        _prefixAreas = new int[RectangleCount];
        _points = new int[PickCalls][];
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
    public int[][] LinearScan()
    {
        var random = new Random(1);

        for (var call = 0; call < PickCalls; call++)
        {
            _points[call] =
                RandomPointInNonOverlappingRectanglesSolution.PickByLinearScan(_rects, _prefixAreas, random);
        }

        return _points;
    }

    [Benchmark]
    public int[][] BinarySearchUpperBound()
    {
        var random = new Random(1);

        for (var call = 0; call < PickCalls; call++)
        {
            _points[call] = RandomPointInNonOverlappingRectanglesSolution.PickByBinarySearchUpperBound(
                _rects, _prefixAreas, random);
        }

        return _points;
    }
}
