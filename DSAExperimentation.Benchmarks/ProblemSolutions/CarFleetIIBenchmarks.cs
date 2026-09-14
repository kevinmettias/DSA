using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CarFleetII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CarFleetIISolution's, the same methods CarFleetIITests
// proves correct, and both take LeetCode's own cars array, so the fleet is built once
// in [GlobalSetup] rather than inside either measured call.
//
// Speeds are strictly increasing front-to-back (car i is always slower than every car
// ahead of it), so no car ever catches up - the classic "never breaks early"
// adversarial input that forces the baseline's forward scan all the way to the end
// for every car, while the sweep still pays one push and at most one pop each.
[MemoryDiagnoser]
public class CarFleetIIBenchmarks
{
    // Gap between adjacent cars' starting positions in the generated fleet.
    private const int PositionSpacing = 10;

    [Params(200, 5_000)]
    public int Length;

    private int[][] _cars = null!;

    [GlobalSetup]
    public void Setup()
    {
        _cars = new int[Length][];

        for (var i = 0; i < Length; i++)
        {
            _cars[i] = [i * PositionSpacing, i + 1];
        }
    }

    [Benchmark(Baseline = true)]
    public double[] BruteForcePerCar() => CarFleetIISolution.GetCollisionTimesByBruteForce(_cars);

    [Benchmark]
    public double[] MonotonicStackSweep() => CarFleetIISolution.GetCollisionTimesByMonotonicStack(_cars);
}
