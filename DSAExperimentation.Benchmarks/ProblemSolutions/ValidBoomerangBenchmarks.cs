using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidBoomerang;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidBoomerangSolution's - the floating-point
// Heron-area check (which needs an epsilon tolerance) against the exact-integer 2D
// cross product (a single multiply-subtract, no floating point at all). Both are
// O(1) per triple, so the loop over _triples is purely the workload: LC 1037's own
// answer is per triple, and counting the boomerangs is only how this harness
// consumes it.
//
// _triples deliberately includes near-collinear (but not exactly collinear) points
// so the floating-point arm's epsilon comparison does real work instead of
// trivially separating from the exact-collinear cases.
[MemoryDiagnoser]
public class ValidBoomerangBenchmarks
{
    private const int RandomSeed = 1037; // LC problem number
    private const int CoordinateBound = 1_000;
    private const int OffsetBound = 10;
    private const int AlternationModulus = 2;
    private const int Point3DisplacementMultiplier = 2;

    private int[][][] _triples = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _triples = new int[Length][][];

        for (var i = 0; i < Length; i++)
        {
            var x1 = random.Next(-CoordinateBound, CoordinateBound);
            var y1 = random.Next(-CoordinateBound, CoordinateBound);
            var dx = random.Next(-OffsetBound, OffsetBound + 1);
            var dy = random.Next(-OffsetBound, OffsetBound + 1);

            // Point 3 sits near the line through points 1 and 2 (one unit off,
            // half the time exactly on it), so the epsilon check and the exact
            // cross product both have a genuinely close call to resolve.
            var x2 = x1 + dx;
            var y2 = y1 + dy;
            var liesExactlyOnLine = i % AlternationModulus == 0;
            var offset = liesExactlyOnLine ? 0 : 1;
            var x3 = x1 + (Point3DisplacementMultiplier * dx) + offset;
            var y3 = y1 + (Point3DisplacementMultiplier * dy);

            _triples[i] = [[x1, y1], [x2, y2], [x3, y3]];
        }
    }

    [Benchmark(Baseline = true)]
    public int FloatingPointAreaCheck()
    {
        var boomerangs = 0;

        foreach (var points in _triples)
        {
            if (ValidBoomerangSolution.IsBoomerangByHeronArea(points))
            {
                boomerangs++;
            }
        }

        return boomerangs;
    }

    [Benchmark]
    public int IntegerCrossProduct()
    {
        var boomerangs = 0;

        foreach (var points in _triples)
        {
            if (ValidBoomerangSolution.IsBoomerangByCrossProduct(points))
            {
                boomerangs++;
            }
        }

        return boomerangs;
    }
}
