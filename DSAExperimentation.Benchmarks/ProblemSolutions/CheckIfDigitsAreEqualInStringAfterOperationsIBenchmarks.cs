using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfDigitsAreEqualInStringAfterOperationsISolution's,
// the same methods CheckIfDigitsAreEqualInStringAfterOperationsITests proves
// correct. Neither strategy has anything worth hoisting out of the measured call -
// parsing a <=10-character digit string is not meaningfully separable preprocessing
// - so [GlobalSetup] only builds the workload string.
[MemoryDiagnoser]
public class CheckIfDigitsAreEqualInStringAfterOperationsIBenchmarks
{
    // LC problem number, reused as the deterministic digit-string seed.
    private const int DigitSeed = 3461;

    // LC's own bound: 3 <= s.Length <= 10.
    [Params(3, 10)]
    public int Length;

    private string _digits = null!;

    [GlobalSetup]
    public void Setup() => _digits = DigitStringWorkloads.BuildDigits(Length, seed: DigitSeed);

    [Benchmark(Baseline = true)]
    public bool AdjacentSumReduction() =>
        CheckIfDigitsAreEqualInStringAfterOperationsISolution.AreEqualByAdjacentSumReduction(_digits);

    [Benchmark]
    public bool PascalRowCoefficients() =>
        CheckIfDigitsAreEqualInStringAfterOperationsISolution.AreEqualByPascalRowCoefficients(_digits);
}
