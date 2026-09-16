using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PowerOfFour;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PowerOfFourSolution's, the same methods
// PowerOfFourTests proves correct.
[MemoryDiagnoser]
public class PowerOfFourBenchmarks
{
    [Params(1073741824, 1073741823)] // 4^15 (a true power of four) vs. one less (not)
    public int Value { get; set; }

    [Benchmark(Baseline = true)]
    public bool IsPowerOfFourByDivisionLoop() => PowerOfFourSolution.IsPowerOfFourByDivisionLoop(Value);

    [Benchmark]
    public bool IsPowerOfFourByBinarySearch() => PowerOfFourSolution.IsPowerOfFourByBinarySearch(Value);
}
