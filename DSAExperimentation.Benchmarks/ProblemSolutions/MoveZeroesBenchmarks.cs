using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MoveZeroes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MoveZeroesSolution's, the same methods
// MoveZeroesTests proves correct. _values is deliberately front-loaded with
// zeroes so the linear-scan strategy's "how far to the next nonzero" grows
// every iteration instead of finding one immediately, forcing its real
// worst-case cost.
[MemoryDiagnoser]
public class MoveZeroesBenchmarks
{
    private const int FrontHalfDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i < Length / FrontHalfDivisor ? 0 : i + 1).ToArray();

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        var nums = _values.ToArray();

        MoveZeroesSolution.MoveZeroesToEndByLinearScan(nums);

        return nums[0];
    }

    [Benchmark]
    public int ArrayIndexedTwoPointer()
    {
        var nums = _values.ToArray();

        MoveZeroesSolution.MoveZeroesToEndByArrayIndexedTwoPointer(nums);

        return nums[0];
    }
}
