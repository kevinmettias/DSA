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

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => IsInFrontHalf(i) ? 0 : NonZeroValueAt(i)).ToArray();

    private bool IsInFrontHalf(int index) => index < Length / FrontHalfDivisor;

    private static int NonZeroValueAt(int index) => index + 1;

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
