using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ExpressionAddOperators;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Expression Add Operators (LC 282): a hand-rolled recursive backtrack vs. this
// repo's own DepthFirstSearch.Traverse walking the identical implicit graph of
// partial expressions (see ExpressionAddOperatorsTests). Unlike the pre-refactor
// version, which counted matches rather than building them, both arms now return
// LeetCode's actual answer - the same methods the tests prove correct. Target is
// unreachable so neither strategy ever short-circuits, and both explore the same
// O(4^n) state space - so this is a constant-factor/allocation comparison, not an
// asymptotic-class split, the same framing IntegerToEnglishWordsBenchmarks uses for
// a bounded domain.
[MemoryDiagnoser]
public class ExpressionAddOperatorsBenchmarks
{
    private const int UnreachableTarget = int.MinValue;

    [Params("1234567", "123456789")]
    public string Num = "";

    [Benchmark(Baseline = true)]
    public int RecursiveBacktrack() =>
        ExpressionAddOperatorsSolution.AddOperatorsByBacktracking(Num, UnreachableTarget).Count;

    [Benchmark]
    public int TraverseComposed() =>
        ExpressionAddOperatorsSolution.AddOperatorsByDepthFirstSearchTraverse(Num, UnreachableTarget).Count;
}
