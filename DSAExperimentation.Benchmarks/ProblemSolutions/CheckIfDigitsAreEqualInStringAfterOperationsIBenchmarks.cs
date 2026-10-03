using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfDigitsAreEqualInStringAfterOperationsISolution's,
// the same methods CheckIfDigitsAreEqualInStringAfterOperationsISolutionTests proves
// correct. Neither strategy has anything worth hoisting out of the measured call -
// parsing a <=100-character digit string is not meaningfully separable preprocessing
// - so [GlobalSetup] only builds the workload string.
public class CheckIfDigitsAreEqualInStringAfterOperationsIBenchmarks
{
    // LC problem number, reused as the deterministic digit-string seed.
    private const int DigitSeed = 3461;

    private string _digits = "";

    // LC's own bound is 3 <= s.Length <= 100. The top size is the one that once broke the
    // Pascal-row arm, whose exact binomials overflowed an int from length 36 on.
    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _digits = DigitStringWorkloads.BuildDigits(Length, seed: DigitSeed);

    [Benchmark(Baseline = true)]
    public bool IsEqualByAdjacentSumReduction() =>
        CheckIfDigitsAreEqualInStringAfterOperationsISolution.IsEqualByAdjacentSumReduction(_digits);

    [Benchmark]
    public bool IsEqualByPascalRowCoefficients() =>
        CheckIfDigitsAreEqualInStringAfterOperationsISolution.IsEqualByPascalRowCoefficients(_digits);
}
