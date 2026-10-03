using DSAExperimentation.LeetCode.ErectTheFence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ErectTheFenceSolution's, the same methods
// ErectTheFenceSolutionTests proves correct. Points are uniform-random in a bounded grid,
// so the hull stays a small fraction of Length (h << n), which is exactly what
// makes the O(n log n + h*n) primitive-based strategy beat the O(n^3) baseline so
// decisively. LC 587 keeps every coordinate in 0..100 and every tree on a position
// of its own, so a draw that repeats an earlier position is skipped.
public class ErectTheFenceBenchmarks
{
    private const int RandomSeed = 587; // LC problem number

    // One past LC 587's largest coordinate, 100.
    private const int CoordinateUpperBoundExclusive = 101;

    private (int X, int Y)[] _points = [];

    [Params(50, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var drawn = new HashSet<(int X, int Y)>();
        var points = new List<(int X, int Y)>(Length);

        // Ends: Length is at most 300, far below the 101 * 101 positions there are to draw.
        while (points.Count < Length)
        {
            var point = (random.Next(0, CoordinateUpperBoundExclusive), random.Next(0, CoordinateUpperBoundExclusive));

            if (drawn.Add(point))
            {
                points.Add(point);
            }
        }

        _points = [.. points];
    }

    [Benchmark(Baseline = true)]
    public List<(int X, int Y)> EveryPairHalfPlaneScan() =>
        ErectTheFenceSolution.OuterTreesByBruteForceHalfPlaneScan(_points);

    [Benchmark]
    public List<(int X, int Y)> MonotoneChainThenEdgeScan() =>
        ErectTheFenceSolution.OuterTreesByMonotoneChain(_points);
}
