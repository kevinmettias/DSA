using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.GenerateRandomPointInACircle.GenerateRandomPointInACircleSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GenerateRandomPointInACircleSolution's, the same
// classes GenerateRandomPointInACircleTests proves correct. Rejection sampling
// over the bounding square (discarding roughly 1 - pi/4 (~21%) of draws) vs. the
// closed-form single-draw polar transform that always lands inside on the first
// try. Each [Benchmark] arm builds its own fresh instance (mirroring how a real
// caller constructs a Solution once) before replaying Draws RandPoint() calls,
// summing the X coordinate so the JIT can't eliminate the replay as dead code.
[MemoryDiagnoser]
public class GenerateRandomPointInACircleBenchmarks
{
    private const double Radius = 10.0;
    private const double XCenter = 5.0;
    private const double YCenter = -3.0;

    [Params(1_000, 100_000)]
    public int Draws;

    [Benchmark(Baseline = true)]
    public double RejectionSampling() =>
        Replay(new GenerateRandomPointInACircleByRejectionSampling(Radius, XCenter, YCenter));

    [Benchmark]
    public double ClosedFormPolar() =>
        Replay(new GenerateRandomPointInACircleByClosedFormPolar(Radius, XCenter, YCenter));

    private double Replay(IRandomPointGenerator generator)
    {
        var sumX = 0.0;

        for (var i = 0; i < Draws; i++)
        {
            sumX += generator.RandPoint()[0];
        }

        return sumX;
    }
}
