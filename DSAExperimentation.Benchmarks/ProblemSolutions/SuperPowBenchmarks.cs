using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SuperPow;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SuperPowSolution's, the same methods SuperPowTests
// proves correct. Exponent is kept to a size the naive strategy can still finish
// (LeetCode's real inputs go up to a 2000-digit exponent - far beyond any naive loop,
// which is exactly why the digit-wise algorithm exists at all).
[MemoryDiagnoser]
public class SuperPowBenchmarks
{
    private const int Base = 7;

    [Params(10_000, 1_000_000)]
    public int Exponent;

    private int[] _digits = null!;

    [GlobalSetup]
    public void Setup() => _digits = Exponent.ToString().Select(c => c - '0').ToArray();

    [Benchmark(Baseline = true)]
    public int RepeatedModularMultiplication() => SuperPowSolution.SuperPowByRepeatedMultiplication(Base, _digits);

    [Benchmark]
    public int HornerSquaring() => SuperPowSolution.SuperPowByHornerSquaring(Base, _digits);
}
