using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CheckIfDigitsAreEqualInStringAfterOperationsIISolution's, the same methods
// CheckIfDigitsAreEqualInStringAfterOperationsIITests proves correct.
[MemoryDiagnoser]
public class CheckIfDigitsAreEqualInStringAfterOperationsIIBenchmarks
{
    private const int RandomSeed = 3463;

    private string _digits = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('0' + random.Next(10));
        }

        _digits = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public bool IsEqualByAdjacentSumReduction() =>
        CheckIfDigitsAreEqualInStringAfterOperationsIISolution.IsEqualByAdjacentSumReduction(_digits);

    [Benchmark]
    public bool IsEqualByLucasBinomialCoefficients() =>
        CheckIfDigitsAreEqualInStringAfterOperationsIISolution.IsEqualByLucasBinomialCoefficients(_digits);
}
