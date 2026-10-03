using DSAExperimentation.LeetCode.MinimumCutsToDivideACircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCutsToDivideACircleSolution's, the same
// methods MinimumCutsToDivideACircleSolutionTests proves correct. SimulateOneCutAtATime
// places one cut at a time, O(n); ClosedFormParityCheck reads the same count off
// n's parity, O(1). Slices stops at LeetCode's own n <= 100 bound, where the linear
// arm's growth barely shows.
public class MinimumCutsToDivideACircleBenchmarks
{
    [Params(11, 100)]
    public int Slices { get; set; }

    [Benchmark(Baseline = true)]
    public int SimulateOneCutAtATime() =>
        MinimumCutsToDivideACircleSolution.NumberOfCutsBySimulation(Slices);

    [Benchmark]
    public int ClosedFormParityCheck() =>
        MinimumCutsToDivideACircleSolution.NumberOfCutsByClosedFormParity(Slices);
}
