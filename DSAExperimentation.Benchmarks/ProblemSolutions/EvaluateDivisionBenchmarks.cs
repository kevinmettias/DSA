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

    [Params(200, 5_000)]
    public int VariableCount;

    private (string Dividend, string Divisor, double Value)[] _equations = null!;
    private string _firstVariable = null!;
    private string _lastVariable = null!;

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
        EvaluateDivisionSolution.EvaluateByDictionaryDfs(_equations, _firstVariable, _lastVariable);

    [Benchmark]
    public double HashMapStackComposed() =>
        EvaluateDivisionSolution.EvaluateByHashMapStackDfs(_equations, _firstVariable, _lastVariable);
}
