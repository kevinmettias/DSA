using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ParsingABooleanExpression;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ParsingABooleanExpressionSolution's. Depth is the
// height of the full operator tree ParsingABooleanExpressionWorkloads builds, so the
// expression grows with it - about 1,500 characters at 8 and 12,000 at 11, the
// deepest full tree inside LC 1106's 2 * 10^4.
public class ParsingABooleanExpressionBenchmarks
{
    private const int RandomSeed = 1106; // LC problem number

    private string _expression = "";

    [Params(8, 11)]
    public int Depth { get; set; }

    [GlobalSetup]
    public void Setup() => _expression = ParsingABooleanExpressionWorkloads.Build(Depth, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public bool IsBoolExprTrueByRecursiveDescent() =>
        ParsingABooleanExpressionSolution.IsBoolExprTrueByRecursiveDescent(_expression);

    [Benchmark]
    public bool IsBoolExprTrueByParserStack() =>
        ParsingABooleanExpressionSolution.IsBoolExprTrueByParserStack(_expression);
}
