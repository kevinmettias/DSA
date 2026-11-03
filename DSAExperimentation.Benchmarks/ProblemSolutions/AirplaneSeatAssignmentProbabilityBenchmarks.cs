using DSAExperimentation.LeetCode.Solutions.AirplaneSeatAssignmentProbability;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AirplaneSeatAssignmentProbabilitySolution's, the same
// methods AirplaneSeatAssignmentProbabilitySolutionTests proves correct. The O(n^2) memoized
// probability recursion summing over every shorter seat count vs. the closed-form O(1)
// "1 if n == 1 else 0.5" formula the recursion reduces to.
public class AirplaneSeatAssignmentProbabilityBenchmarks
{
    [Params(100, 1_000)]
    public int PlaneSize { get; set; }

    [Benchmark]
    public double MemoizedRecursion()
        => AirplaneSeatAssignmentProbabilitySolution.NthPersonGetsNthSeatByMemoizedRecursion(PlaneSize);

    [Benchmark(Baseline = true)]
    public double ClosedForm()
        => AirplaneSeatAssignmentProbabilitySolution.NthPersonGetsNthSeatByClosedForm(PlaneSize);
}
