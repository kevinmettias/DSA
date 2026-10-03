using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.EvaluateReversePolishNotation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is EvaluateReversePolishNotationSolution's, the
// same method EvaluateReversePolishNotationSolutionTests proves correct. Pre-migration
// this class was an untested compile-smoke placeholder (`Baseline() => 1`,
// `PrimitiveComposed() => 1`) rather than a second strategy to reconcile.
//
// The expression comes from EvaluateReversePolishNotationWorkloads: a seeded postfix
// expression over every operator that keeps LC 150's promises of no division by zero
// and no intermediate value outside a 32-bit int. OperandCount stops at 5,000, the
// most LC 150's 10,000-token cap admits.
public class EvaluateReversePolishNotationBenchmarks
{
    private const int RandomSeed = 150; // LC problem number

    private string[] _tokens = [];

    [Params(50, 500, 5_000)]
    public int OperandCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _tokens = EvaluateReversePolishNotationWorkloads.Build(OperandCount, new Random(RandomSeed)).Tokens;

    [Benchmark(Baseline = true)]
    public int OperandStack() => EvaluateReversePolishNotationSolution.EvaluateByOperandStack(_tokens);
}
