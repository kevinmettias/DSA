using DSAExperimentation.LeetCode.MoveZeroes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MoveZeroesSolution's, the same methods
// MoveZeroesSolutionTests proves correct. _values is deliberately front-loaded with
// zeroes so the linear-scan strategy's "how far to the next nonzero" grows
// every iteration instead of finding one immediately, forcing its real
// worst-case cost.
public class MoveZeroesBenchmarks
{
    private const int FrontHalfDivisor = 2;

    private int[] _values = [];

    // The array each arm copies _values into and rearranges; allocated once in setup.
    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(0, Length).Select(i => IsInFrontHalf(i) ? 0 : NonZeroValueAt(i)).ToArray();
        _nums = new int[Length];
    }

    private bool IsInFrontHalf(int index) => index < Length / FrontHalfDivisor;

    private static int NonZeroValueAt(int index) => index + 1;

    // Each arm rearranges a fresh copy in place and returns it. The copy is timed on purpose:
    // both strategies mutate their input, so every arm pays the same O(n) copy.
    [Benchmark(Baseline = true)]
    public int[] LinearScan()
    {
        _values.CopyTo(_nums, 0);

        MoveZeroesSolution.MoveZeroesToEndByLinearScan(_nums);

        return _nums;
    }

    [Benchmark]
    public int[] ArrayIndexedTwoPointer()
    {
        _values.CopyTo(_nums, 0);

        MoveZeroesSolution.MoveZeroesToEndByArrayIndexedTwoPointer(_nums);

        return _nums;
    }
}
