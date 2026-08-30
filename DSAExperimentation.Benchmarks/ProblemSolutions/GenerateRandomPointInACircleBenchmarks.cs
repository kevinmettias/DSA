using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Generate Random Point in a Circle (LC 478): rejection sampling over the bounding
// square (loop until a drawn point actually lands inside the circle, discarding
// roughly 1 - pi/4 (~21%) of draws) vs. the closed-form single-draw polar transform
// (r = radius * sqrt(u), theta = u * 2*PI) that always lands inside on the first try.
// Both draw from a freshly seeded System.Random per call - the ShuffleAnArrayBenchmarks
// precedent - since the problem has no Representation/Topology axis for a repo
// DataStructure to add (ARCHITECTURE.md section 12.2's "Operations + open runtime
// object" shape).
[MemoryDiagnoser]
public class GenerateRandomPointInACircleBenchmarks
{
    private const double Radius = 10.0;
    private const double XCenter = 5.0;
    private const double YCenter = -3.0;

    [Params(1_000, 100_000)]
    public int Draws;

    [Benchmark(Baseline = true)]
    public double RejectionSampling()
    {
        var random = new Random(1);
        var sumX = 0.0;

        for (var i = 0; i < Draws; i++)
        {
            double x, y;
            do
            {
                x = (random.NextDouble() * 2 * Radius) - Radius;
                y = (random.NextDouble() * 2 * Radius) - Radius;
            }
            while ((x * x) + (y * y) > Radius * Radius);

            sumX += XCenter + x;
        }

        return sumX;
    }

    [Benchmark]
    public double ClosedFormPolar()
    {
        var random = new Random(1);
        var sumX = 0.0;

        for (var i = 0; i < Draws; i++)
        {
            var r = Radius * Math.Sqrt(random.NextDouble());
            var angle = random.NextDouble() * 2 * Math.PI;
            sumX += XCenter + (r * Math.Cos(angle));
        }

        return sumX;
    }
}
