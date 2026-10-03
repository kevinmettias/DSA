using DSAExperimentation.LeetCode.CountLatticePointsInsideACircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountLatticePointsInsideACircleSolution's, the same
// methods CountLatticePointsInsideACircleSolutionTests proves agree.
//
// Circles are scattered across a bound far wider than their radius, so the combined
// bounding box the full-grid scan must walk is much larger than the sum of the
// circles' own local boxes - the case where the per-circle scan actually wins
// instead of just adding Set overhead on top of the same amount of work. That bound
// is LC 2249's own: at most 200 circles, centres in [1, 100], and a radius no larger
// than either centre coordinate, so no circle crosses an axis.
public class CountLatticePointsInsideACircleBenchmarks
{
    private const int RandomSeed = 2249; // LC problem number
    private const int MaxCoordinate = 100;
    private const int MaxRadiusExclusive = 6;

    private int[][] _circles = [];

    [Params(50, 200)]
    public int CircleCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _circles = Enumerable.Range(0, CircleCount).Select(_ => DrawCircle(random)).ToArray();
    }

    // One [x, y, r] circle: the centre first, then a radius below MaxRadiusExclusive that
    // stays within min(x, y).
    private static int[] DrawCircle(Random random)
    {
        var x = random.Next(1, MaxCoordinate + 1);
        var y = random.Next(1, MaxCoordinate + 1);
        var nearestAxis = Math.Min(x, y);
        var radiusBoundExclusive = Math.Min(MaxRadiusExclusive, nearestAxis + 1);

        return [x, y, random.Next(1, radiusBoundExclusive)];
    }

    [Benchmark(Baseline = true)]
    public int FullGridScan() =>
        CountLatticePointsInsideACircleSolution.CountLatticePointsByFullGridScan(_circles);

    [Benchmark]
    public int PerCircleBoundingBoxWithSet() =>
        CountLatticePointsInsideACircleSolution.CountLatticePointsByPerCircleSetUnion(_circles);
}
