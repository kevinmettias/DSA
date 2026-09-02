using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.AnglesOfATriangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AnglesOfATriangleSolution's, the same methods
// AnglesOfATriangleTests proves correct. No [Params] axis - the input is always
// exactly three sides, so there is nothing to scale; the comparison is purely
// the constant-factor cost of a third Acos call versus deriving the third angle
// from the triangle's angle sum.
[MemoryDiagnoser]
public class AnglesOfATriangleBenchmarks
{
    private const int Seed = 3899;

    private int[] _sides = null!;

    [GlobalSetup]
    public void Setup() => _sides = TriangleSideWorkloads.BuildValidTriangle(seed: Seed);

    [Benchmark(Baseline = true)]
    public double[] LawOfCosines() => AnglesOfATriangleSolution.InternalAnglesByLawOfCosines(_sides);

    [Benchmark]
    public double[] AngleSum() => AnglesOfATriangleSolution.InternalAnglesByAngleSum(_sides);
}
