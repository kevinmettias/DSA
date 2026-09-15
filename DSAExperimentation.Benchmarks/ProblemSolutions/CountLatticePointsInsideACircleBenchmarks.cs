using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountLatticePointsInsideACircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountLatticePointsInsideACircleSolution's, the same
// methods CountLatticePointsInsideACircleTests proves agree.
//
// Circles are scattered across a bound far wider than their radius, so the combined
// bounding box the full-grid scan must walk is much larger than the sum of the
// circles' own local boxes - the case where the per-circle scan actually wins
// instead of just adding Set overhead on top of the same amount of work.
[MemoryDiagnoser]
public class CountLatticePointsInsideACircleBenchmarks
{
    private const int RandomSeed = 2249; // LC problem number
    private const int CoordinateBound = 150;
    private const int MaxRadiusExclusive = 6;

    private int[][] _circles = [];

    [Params(50, 300)]
    public int CircleCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _circles = Enumerable.Range(0, CircleCount)
            .Select(_ => new[]
            {
                random.Next(-CoordinateBound, CoordinateBound),
                random.Next(-CoordinateBound, CoordinateBound),
                random.Next(1, MaxRadiusExclusive),
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullGridScan() =>
        CountLatticePointsInsideACircleSolution.CountLatticePointsByFullGridScan(_circles);

    [Benchmark]
    public int PerCircleBoundingBoxWithSet() =>
        CountLatticePointsInsideACircleSolution.CountLatticePointsByPerCircleSetUnion(_circles);
}
