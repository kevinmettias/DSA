using DSAExperimentation.LeetCode.NumberOfCommonFactors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfCommonFactorsSolution's, the same methods
// NumberOfCommonFactorsSolutionTests proves correct. _first/_second are random within
// [Magnitude/2, Magnitude], so their gcd stays small relative to Magnitude on
// average - exactly the shape where the sqrt(gcd) reduction pays off over the
// min(a, b) baseline. Magnitude stops at LC 2427's own bound of a, b <= 1,000.
public class NumberOfCommonFactorsBenchmarks
{
    private const int RandomSeed = 2427; // LC problem number

    // Operands are drawn from the top half of [0, Magnitude].
    private const int TopHalfDivisor = 2;

    private int _first;
    private int _second;

    [Params(100, 1_000)]
    public int Magnitude { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _first = random.Next(Magnitude / TopHalfDivisor, Magnitude + 1);
        _second = random.Next(Magnitude / TopHalfDivisor, Magnitude + 1);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => NumberOfCommonFactorsSolution.CountCommonFactorsByLinearScan(_first, _second);

    [Benchmark]
    public int DivisorEnumeration()
        => NumberOfCommonFactorsSolution.CountCommonFactorsByDivisorEnumeration(_first, _second);
}
