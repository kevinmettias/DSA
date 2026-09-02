using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BasicCalculator;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BasicCalculatorSolution's, the same methods
// BasicCalculatorTests proves correct. The generated expression chains many
// sequential, only single-level-nested "+(a+b)"/"-(a+b)" groups, so
// RecursiveDescent's recursion depth stays constant (~2) as Length grows
// instead of risking a StackOverflowException.
[MemoryDiagnoser]
public class BasicCalculatorBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() => _expression = BasicCalculatorWorkloads.BuildExpression(Length);

    [Benchmark(Baseline = true)]
    public int RecursiveDescent() => BasicCalculatorSolution.CalculateByRecursiveDescent(_expression);

    [Benchmark]
    public int StackScan() => BasicCalculatorSolution.CalculateByStackScan(_expression);
}
