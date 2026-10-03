using DSAExperimentation.LeetCode.GenerateRandomPointInACircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GenerateRandomPointInACircleSolution's, the same
// classes GenerateRandomPointInACircleSolutionTests proves correct. Rejection sampling
// over the bounding square (discarding roughly 1 - pi/4 (~21%) of draws) vs. the
// closed-form single-draw polar transform that always lands inside on the first
// try. Each [Benchmark] arm builds its own fresh instance (mirroring how a real
// caller constructs a Solution once) before replaying Draws RandPoint() calls,
// and returns every point drawn so the JIT can't eliminate the replay as dead code.
// Every instance is built with the same DrawSeed, so a rebuilt harness replays
// the same points; the two arms still answer differently, because rejection
// sampling spends a variable number of draws per point and the polar form two.
public class GenerateRandomPointInACircleBenchmarks
{
    private const double Radius = 10.0;
    private const double XCenter = 5.0;
    private const double YCenter = -3.0;
    private const int DrawSeed = 478; // LC problem number

    // Every point RandPoint() draws, in order; sized in setup so the replay allocates
    // nothing beyond what the generator itself returns.
    private double[][] _points = [];

    [Params(1_000, 100_000)]
    public int Draws { get; set; }

    [GlobalSetup]
    public void Setup() => _points = new double[Draws][];

    [Benchmark(Baseline = true)]
    public double[][] RejectionSampling() =>
        Replay(new GenerateRandomPointInACircleSolution.GenerateRandomPointInACircleByRejectionSampling(
            Radius, XCenter, YCenter, DrawSeed));

    [Benchmark]
    public double[][] ClosedFormPolar() =>
        Replay(new GenerateRandomPointInACircleSolution.GenerateRandomPointInACircleByClosedFormPolar(
            Radius, XCenter, YCenter, DrawSeed));

    private double[][] Replay(GenerateRandomPointInACircleSolution.IRandomPointGenerator generator)
    {
        for (var i = 0; i < Draws; i++)
        {
            _points[i] = generator.RandPoint();
        }

        return _points;
    }
}
