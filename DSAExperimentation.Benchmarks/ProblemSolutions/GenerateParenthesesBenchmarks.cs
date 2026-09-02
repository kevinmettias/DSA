using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GenerateParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GenerateParenthesesSolution's.
[MemoryDiagnoser]
public class GenerateParenthesesBenchmarks
{
    [Params(5, 8)]
    public int Pairs;

    [Benchmark(Baseline = true)]
    public List<string> RecursiveSpecialized() => GenerateParenthesesSolution.GenerateByRecursiveSpecialized(Pairs);

    [Benchmark]
    public List<string> Backtracking() => GenerateParenthesesSolution.GenerateByBacktracking(Pairs);
}
