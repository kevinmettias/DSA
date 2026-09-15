using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SolveTheEquation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SolveTheEquationSolution's, the same methods
// SolveTheEquationTests proves correct.
[MemoryDiagnoser]
public class SolveTheEquationBenchmarks
{
    // Arbitrary seed for reproducible benchmark input.
    private const int RandomSeed = 11;

    // Random.Next(BinaryChoiceBound) yields 0 or 1, used for coin-flip decisions
    // (term sign, whether a term carries the 'x' variable).
    private const int BinaryChoiceBound = 2;

    // Exclusive upper bound on a generated term's numeric coefficient magnitude.
    private const int CoefficientUpperBound = 100;

    private string _equation = "";

    [Params(200, 5_000)]
    public int TermsPerSide { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _equation = $"{BuildSide(random, TermsPerSide)}={BuildSide(random, TermsPerSide)}";
    }

    private static string BuildSide(Random random, int termCount)
    {
        var side = new StringBuilder();

        for (var t = 0; t < termCount; t++)
        {
            if (t > 0)
            {
                var isPlus = random.Next(BinaryChoiceBound) == 0;
                side.Append(isPlus ? '+' : '-');
            }

            var coefficient = random.Next(1, CoefficientUpperBound);
            side.Append(coefficient);

            if (random.Next(BinaryChoiceBound) == 0)
            {
                side.Append('x');
            }
        }

        return side.ToString();
    }

    [Benchmark(Baseline = true)]
    public string SubstringParse() => SolveTheEquationSolution.SolveBySubstringParse(_equation);

    [Benchmark]
    public string SpanParse() => SolveTheEquationSolution.SolveBySpanParse(_equation);
}
