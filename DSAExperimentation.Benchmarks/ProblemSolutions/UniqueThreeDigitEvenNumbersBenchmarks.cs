using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.UniqueThreeDigitEvenNumbers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniqueThreeDigitEvenNumbersSolution's, the same
// methods UniqueThreeDigitEvenNumbersTests proves correct. LC caps digits.length at
// 10, so [Params] stays within that range rather than the larger sizes this
// project's other benchmarks use.
[MemoryDiagnoser]
public class UniqueThreeDigitEvenNumbersBenchmarks
{
    private const int Seed = 3483;
    private const int DigitUpperBound = 10;

    [Params(5, 10)]
    public int DigitCount;

    private int[] _digits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _digits = Enumerable.Range(0, DigitCount).Select(_ => random.Next(0, DigitUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int IndexPermutationScan() => UniqueThreeDigitEvenNumbersSolution.CountByIndexPermutationScan(_digits);

    [Benchmark]
    public int Backtracking() => UniqueThreeDigitEvenNumbersSolution.CountByBacktracking(_digits);
}
