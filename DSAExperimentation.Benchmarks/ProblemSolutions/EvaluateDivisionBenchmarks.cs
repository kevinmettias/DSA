using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.EvaluateDivision;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are EvaluateDivisionSolution's, the same methods
// EvaluateDivisionTests proves correct. The variables form one long chain (v0/v1 =
// 2.0, v1/v2 = 2.0, ...) so every query below walks the full chain instead of an
// early exit.
[MemoryDiagnoser]
public class EvaluateDivisionBenchmarks
{
    private const double EdgeWeight = 2.0;
    private const string FirstVariableName = "v0";

    private (string Dividend, string Divisor, double Value)[] _equations = [];

    private string _firstVariable = "";
    private string _lastVariable = "";
    [Params(200, 5_000)]
    public int VariableCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _equations = new (string, string, double)[VariableCount - 1];

        for (var i = 0; i < VariableCount - 1; i++)
        {
            _equations[i] = ($"v{i}", $"v{i + 1}", EdgeWeight);
        }

        _firstVariable = FirstVariableName;
        _lastVariable = $"v{VariableCount - 1}";
    }

    [Benchmark(Baseline = true)]
    public double DictionaryBased() =>
        EvaluateDivisionSolution.EvaluateByDictionaryDfs(
            _equations,
            new EvaluateDivisionSolution.Dividend(_firstVariable),
            new EvaluateDivisionSolution.Divisor(_lastVariable));

    [Benchmark]
    public double HashMapStackComposed() =>
        EvaluateDivisionSolution.EvaluateByHashMapStackDfs(
            _equations,
            new EvaluateDivisionSolution.Dividend(_firstVariable),
            new EvaluateDivisionSolution.Divisor(_lastVariable));
}
