using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SqrtXSolution's - integer Newton's method against the
// library's binary search - and neither uses the exponent or root operator LC 69 forbids.
public class SqrtXBenchmarks
{
    [Params(10_000, int.MaxValue)]
    public int Value { get; set; }

    [Benchmark(Baseline = true)]
    public int NewtonIteration() => SqrtXSolution.RootByNewtonIteration(Value);

    [Benchmark]
    public int BinarySearchRoot() => SqrtXSolution.RootByBinarySearch(Value);
}
