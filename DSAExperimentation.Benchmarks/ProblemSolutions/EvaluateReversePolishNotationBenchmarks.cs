using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.EvaluateReversePolishNotation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is EvaluateReversePolishNotationSolution's, the
// same method EvaluateReversePolishNotationTests proves correct. Pre-migration
// this class was an untested compile-smoke placeholder (`Baseline() => 1`,
// `PrimitiveComposed() => 1`) rather than a second strategy to reconcile.
[MemoryDiagnoser]
public class EvaluateReversePolishNotationBenchmarks
{
    // LeetCode 150's own third example: a fully nested expression that
    // exercises every operator rather than just addition.
    private static readonly string[] Tokens =
        ["10", "6", "9", "3", "+", "-11", "*", "/", "*", "17", "+", "5", "+"];

    [Benchmark(Baseline = true)]
    public int OperandStack() => EvaluateReversePolishNotationSolution.EvalByOperandStack(Tokens);
}
