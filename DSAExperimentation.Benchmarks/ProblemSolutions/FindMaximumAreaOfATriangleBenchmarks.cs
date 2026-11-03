using DSAExperimentation.LeetCode.Solutions.FindMaximumAreaOfATriangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMaximumAreaOfATriangleSolution's, the same
// methods CatalogCaseTests proves correct. Coordinates are drawn
// from a grid narrower than the point count (LargestTriangleAreaBenchmarks' own
// point-generation precedent, tightened here) so rows and columns collide and
// both arms have real axis-aligned triangles to score, not just -1 every run.
public class FindMaximumAreaOfATriangleBenchmarks
{
    private const int Seed = 3588;

    private int[][] _coords = [];

    [Params(60, 300)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var bound = Math.Max(10, PointCount / 3);
        // Kept in the order drawn, the set only checking membership: a HashSet's order follows the
        // points' hashes, which .NET seeds afresh in every process, so taking its order would give each
        // benchmark process a different workload from the same seed.
        var seen = new HashSet<(int X, int Y)>();
        var drawn = new List<int[]>(PointCount);

        while (drawn.Count < PointCount)
        {
            var point = (X: random.Next(1, bound), Y: random.Next(1, bound));
            var isNewPoint = seen.Add(point);

            if (isNewPoint)
            {
                drawn.Add([point.X, point.Y]);
            }
        }

        _coords = [.. drawn];
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => FindMaximumAreaOfATriangleSolution.MaxAreaByBruteForce(_coords);

    [Benchmark]
    public long SpreadHashMap() => FindMaximumAreaOfATriangleSolution.MaxAreaBySpreadHashMap(_coords);
}
