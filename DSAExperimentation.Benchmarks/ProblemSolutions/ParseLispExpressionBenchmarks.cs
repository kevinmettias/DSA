using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ParseLispExpression;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ParseLispExpressionSolution's. Parse Lisp
// Expression (LC 736) - a baseline that gives every nested "let" its own
// full copy of the enclosing scope (an easy-to-reach-for interpreter design,
// but O(scope size) extra work per "let" on top of evaluating it) vs. this
// repo's own SinglyLinkedListNode<T> chaining per-"let" HashMap<string,long>
// scopes together (O(1) extra per "let", lookup walks Next). _expression
// chains Length nested "(let vI I ...)" scopes so the environment strategy
// gets exercised at every depth. Allocated is the fairer column to read here
// rather than Mean: both variants pay the same O(depth^2) string-slicing
// cost this simple recursive-descent parser re-pays at every nesting level
// regardless of environment strategy (each level re-strips and re-tokenizes
// its own shrinking substring), so that cost dominates wall-clock time for
// both. ScopeChain's allocation total still comes in lower at every size
// because it alone avoids also duplicating every prior binding on top of it.
[MemoryDiagnoser]
public class ParseLispExpressionBenchmarks
{
    private const string LetExpressionPrefix = "(let v";
    private const int PenultimateIndexOffset = 2;

    [Params(50, 400)]
    public int Length;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() => _expression = BuildNestedLetExpression(Length);

    [Benchmark(Baseline = true)]
    public long CopyEnvironmentPerLet() => ParseLispExpressionSolution.EvaluateByCopiedScope(_expression);

    [Benchmark]
    public long ScopeChain() => ParseLispExpressionSolution.EvaluateByScopeChain(_expression);

    // Builds "(let v0 0 (let v1 1 (let v2 2 ... (add v0 (add v1 v2))...)))" -
    // Length nested lets, each shadowing nothing, wrapping a right-nested sum
    // of every bound variable.
    private static string BuildNestedLetExpression(int depth)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < depth; i++)
        {
            builder.Append(LetExpressionPrefix).Append(i).Append(' ').Append(i).Append(' ');
        }

        builder.Append(BuildNestedSum(depth));
        builder.Append(')', depth);

        return builder.ToString();
    }

    private static string BuildNestedSum(int depth)
    {
        var expr = $"v{depth - 1}";

        for (var i = depth - PenultimateIndexOffset; i >= 0; i--)
        {
            expr = $"(add v{i} {expr})";
        }

        return expr;
    }
}
