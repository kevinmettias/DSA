using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Cuts to Divide a Circle (LC 2481): an O(n) simulation that places one cut at
// a time - a diameter cut removes 2 remaining slices at once when n is even, a radius
// cut removes 1 when n is odd - vs. the O(1) closed-form parity check the simulation
// always reduces to. No repo data-structure primitive fits this problem (see
// MinimumCutsToDivideACircleTests.cs: it is pure arithmetic with no recursive/search
// structure to compose over), so both variants live here directly rather than routing
// through a primitive that would not change either one's behavior.
[MemoryDiagnoser]
public class MinimumCutsToDivideACircleBenchmarks
{
    [Params(101, 100_001)]
    public int Slices;

    [Benchmark(Baseline = true)]
    public int SimulateOneCutAtATime()
    {
        if (Slices == 1)
        {
            return 0;
        }

        var remaining = Slices;
        var step = remaining % 2 == 0 ? 2 : 1;
        var cuts = 0;

        while (remaining > 0)
        {
            remaining -= step;
            cuts++;
        }

        return cuts;
    }

    [Benchmark]
    public int ClosedFormParityCheck()
        => Slices == 1 ? 0 : Slices % 2 == 0 ? Slices / 2 : Slices;
}
