using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SolveTheEquation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SolveTheEquationSolution's, the same methods
// SolveTheEquationSolutionTests proves correct. SolveTheEquationWorkloads plants an integer
// solution, as LC 640 promises every single solution is, and TermsPerSide stops at 125, the
// most terms a side its four-character terms fit in LC 640's 1,000-character equation.
public class SolveTheEquationBenchmarks
{
    // Arbitrary seed for reproducible benchmark input.
    private const int RandomSeed = 11;

    private string _equation = "";

    [Params(12, 125)]
    public int TermsPerSide { get; set; }

    [GlobalSetup]
    public void Setup() => _equation = SolveTheEquationWorkloads.Build(TermsPerSide, RandomSeed);

    [Benchmark(Baseline = true)]
    public string SubstringParse() => SolveTheEquationSolution.SolveBySubstringParse(_equation);

    [Benchmark]
    public string SpanParse() => SolveTheEquationSolution.SolveBySpanParse(_equation);
}
