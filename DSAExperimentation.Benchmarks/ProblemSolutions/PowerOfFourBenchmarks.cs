using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PowerOfFour;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PowerOfFourSolution's, the same methods
// PowerOfFourTests proves correct.
[MemoryDiagnoser]
public class PowerOfFourBenchmarks
{
    [Params(1073741824, 1073741823)] // 4^15 (a true power of four) vs. one less (not)
    public int Value;

    [Benchmark(Baseline = true)]
    public bool DivisionLoop() => PowerOfFourSolution.IsPowerOfFourByDivisionLoop(Value);

    [Benchmark]
    public bool BinarySearchOverPowers() => PowerOfFourSolution.IsPowerOfFourByBinarySearch(Value);
}
