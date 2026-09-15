using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ParsingABooleanExpression;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ParsingABooleanExpressionSolution's. Depth controls
// how deeply the generated expression nests (and therefore its total size), not the
// value it evaluates to.
[MemoryDiagnoser]
public class ParsingABooleanExpressionBenchmarks
{
    // random.Next(LeafChance) == 0: roughly a 1-in-4 chance to end the generated
    // expression early, independent of remaining depth.
    private const int LeafChance = 4;

    // random.Next(TokenChoiceCount): coin flip between the two leaf tokens below.
    private const int TokenChoiceCount = 2;
    private const string TrueToken = "t";
    private const string FalseToken = "f";

    // random.Next(OperatorChoiceCount): choose among !, &, |.
    private const int OperatorChoiceCount = 3;

    // LC problem number is not used here; the seed only has to be deterministic.
    private const int RandomSeed = 1;

    private string _expression = "";

    [Params(8, 12)]
    public int Depth { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _expression = Generate(random, Depth);
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveDescent() =>
        ParsingABooleanExpressionSolution.ParseBoolExprByRecursiveDescent(_expression);

    [Benchmark]
    public bool StackBased() =>
        ParsingABooleanExpressionSolution.ParseBoolExprByParserStack(_expression);

    // Workload sizing only: how large an expression to measure, not how to evaluate
    // one.
    private static string Generate(Random random, int depth)
    {
        if (depth == 0 || random.Next(LeafChance) == 0)
        {
            return IsTrueToken(random) ? TrueToken : FalseToken;
        }

        return random.Next(OperatorChoiceCount) switch
        {
            0 => $"!({Generate(random, depth - 1)})",
            1 => $"&({Generate(random, depth - 1)},{Generate(random, depth - 1)})",
            _ => $"|({Generate(random, depth - 1)},{Generate(random, depth - 1)})",
        };
    }

    // One draw, taken only when the leaf branch is reached, in the same step order
    // the generated expression is seeded from.
    private static bool IsTrueToken(Random random) => random.Next(TokenChoiceCount) == 0;
}
