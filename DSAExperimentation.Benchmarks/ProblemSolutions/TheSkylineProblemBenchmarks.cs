using DSAExperimentation.LeetCode.TheSkylineProblem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TheSkylineProblemSolution's, the same methods
// TheSkylineProblemSolutionTests proves correct. Buildings are randomly overlapping so
// both strategies pay their full worst-case cost rather than degenerating to
// disjoint ranges, and are handed over sorted by left edge, as LC 218 guarantees.
public class TheSkylineProblemBenchmarks
{
    private const int LeftCoordinateSpreadMultiplier = 2;
    private const int MaxBuildingWidth = 50;
    private const int MaxBuildingHeight = 1_000;
    private const int Seed = 218;

    private int[][] _buildings = [];

    [Params(100, 1_000)]
    public int BuildingCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var buildings = new int[BuildingCount][];

        for (var i = 0; i < BuildingCount; i++)
        {
            var left = random.Next(0, BuildingCount * LeftCoordinateSpreadMultiplier);
            var width = random.Next(1, MaxBuildingWidth);
            var height = random.Next(1, MaxBuildingHeight);
            buildings[i] = [left, left + width, height];
        }

        _buildings = [.. buildings.OrderBy(building => building[0])];
    }

    [Benchmark(Baseline = true)]
    public List<int[]> BruteForceCriticalPoints() => TheSkylineProblemSolution.GetSkylineByBruteForce(_buildings);

    [Benchmark]
    public List<int[]> SweepLineHeap() => TheSkylineProblemSolution.GetSkylineBySweepLineHeap(_buildings);
}
