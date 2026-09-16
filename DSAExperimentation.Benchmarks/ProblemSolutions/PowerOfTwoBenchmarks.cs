using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PowerOfTwo;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PowerOfTwoSolution's, the same methods
// PowerOfTwoTests proves correct. No workload to hoist into [GlobalSetup] - the
// input is a single int, and Value is fixed to the same non-power-of-two operand
// for both arms so neither gets to stop after one iteration/comparison.
[MemoryDiagnoser]
public class PowerOfTwoBenchmarks
{
    private const int Value = 999_999_937; // a large prime, far from any power of two

    [Benchmark(Baseline = true)]
    public bool IsPowerOfTwoByRepeatedDivision() => PowerOfTwoSolution.IsPowerOfTwoByRepeatedDivision(Value);

    [Benchmark]
    public bool IsPowerOfTwoByBitTrick() => PowerOfTwoSolution.IsPowerOfTwoByBitTrick(Value);
}
