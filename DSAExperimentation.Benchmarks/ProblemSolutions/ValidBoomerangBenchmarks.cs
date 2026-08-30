using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Boomerang (LC 1037): a floating-point slope/area check (Heron's formula
// over Math.Sqrt distances, an inexact naive approach real enough to be a common
// first instinct - and one that needs an epsilon tolerance to avoid misclassifying
// near-collinear points) vs. the exact-integer 2D cross product (a single
// multiply-subtract, no floating point at all). Both are O(1) per triple; no repo
// container or algorithm primitive applies here - there is nothing to compose over
// three fixed (x,y) pairs and one closed-form sign check, the same "lighter
// repo-primitive fit" case ComplexNumberMultiplication/MirrorReflection already
// establish. _triples deliberately includes near-collinear (but not exactly
// collinear) points so the naive approach's epsilon comparison does real work
// instead of trivially separating from the exact-collinear cases.
[MemoryDiagnoser]
public class ValidBoomerangBenchmarks
{
    private const double Epsilon = 1e-9;

    [Params(200, 5_000)]
    public int Length;

    private int[][][] _triples = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1037);
        _triples = new int[Length][][];

        for (var i = 0; i < Length; i++)
        {
            var x1 = random.Next(-1_000, 1_000);
            var y1 = random.Next(-1_000, 1_000);
            var dx = random.Next(-10, 11);
            var dy = random.Next(-10, 11);

            // Point 3 sits near the line through points 1 and 2 (one unit off,
            // half the time exactly on it), so the naive epsilon check and the
            // exact cross product both have a genuinely close call to resolve.
            var x2 = x1 + dx;
            var y2 = y1 + dy;
            var offset = i % 2 == 0 ? 0 : 1;
            var x3 = x1 + (2 * dx) + offset;
            var y3 = y1 + (2 * dy);

            _triples[i] = [[x1, y1], [x2, y2], [x3, y3]];
        }
    }

    [Benchmark(Baseline = true)]
    public int FloatingPointAreaCheck()
    {
        var boomerangs = 0;

        foreach (var points in _triples)
        {
            if (IsBoomerangByArea(points))
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
            if (IsBoomerangByCrossProduct(points))
            {
                boomerangs++;
            }
        }

        return boomerangs;
    }

    private static bool IsBoomerangByArea(int[][] points)
    {
        var a = Distance(points[0], points[1]);
        var b = Distance(points[1], points[2]);
        var c = Distance(points[2], points[0]);

        var s = (a + b + c) / 2.0;
        var areaSquared = s * (s - a) * (s - b) * (s - c);

        return areaSquared > Epsilon;
    }

    private static double Distance(int[] p, int[] q)
    {
        var dx = p[0] - q[0];
        var dy = p[1] - q[1];
        return Math.Sqrt((dx * dx) + (dy * dy));
    }

    private static bool IsBoomerangByCrossProduct(int[][] points)
    {
        var (x1, y1) = (points[0][0], points[0][1]);
        var (x2, y2) = (points[1][0], points[1][1]);
        var (x3, y3) = (points[2][0], points[2][1]);

        var cross = ((long)(x2 - x1) * (y3 - y1)) - ((long)(x3 - x1) * (y2 - y1));
        return cross != 0;
    }
}
