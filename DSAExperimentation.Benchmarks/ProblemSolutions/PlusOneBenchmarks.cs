using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PlusOne;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PlusOneSolution's. All-nines input forces a full
// carry cascade, so the array-walk strategy gets no early exit either.
[MemoryDiagnoser]
public class PlusOneBenchmarks
{
    private const int MaxDigitValue = 9; // base-10 digit ceiling; also the worst-case seed that forces a full carry cascade

    private int[] _digits = null!;

    [Params(200, 5_000)]
    public int Length;

    [GlobalSetup]
    public void Setup() => _digits = Enumerable.Repeat(MaxDigitValue, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int[] ArrayWalk() => PlusOneSolution.IncrementByArrayWalk(_digits);

    [Benchmark]
    public int[] DigitStack() => PlusOneSolution.IncrementByDigitStack(_digits);
}
