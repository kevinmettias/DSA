using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumCutsToDivideACircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCutsToDivideACircleSolution's, the same
// methods MinimumCutsToDivideACircleTests proves correct. SimulateOneCutAtATime
// places one cut at a time, O(n); ClosedFormParityCheck reads the same count off
// n's parity, O(1). The sizes deliberately run past LeetCode's own n <= 100 bound
// so the linear arm has something to measure.
[MemoryDiagnoser]
public class MinimumCutsToDivideACircleBenchmarks
{
    [Params(101, 100_001)]
    public int Slices;

    [Benchmark(Baseline = true)]
    public int SimulateOneCutAtATime() =>
        MinimumCutsToDivideACircleSolution.NumberOfCutsBySimulation(Slices);

    [Benchmark]
    public int ClosedFormParityCheck() =>
        MinimumCutsToDivideACircleSolution.NumberOfCutsByClosedFormParity(Slices);
}
