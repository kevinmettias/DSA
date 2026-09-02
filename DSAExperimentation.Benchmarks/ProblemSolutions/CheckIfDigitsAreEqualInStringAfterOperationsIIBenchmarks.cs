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

    [Params(200, 5_000)]
    public int Length;

    private string _digits = null!;

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
    public bool AdjacentSumReduction() =>
        CheckIfDigitsAreEqualInStringAfterOperationsIISolution.AreEqualByAdjacentSumReduction(_digits);

    [Benchmark]
    public bool LucasBinomialCoefficients() =>
        CheckIfDigitsAreEqualInStringAfterOperationsIISolution.AreEqualByLucasBinomialCoefficients(_digits);
}
