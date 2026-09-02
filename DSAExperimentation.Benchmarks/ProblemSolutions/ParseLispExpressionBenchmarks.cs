using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using RepoScopeNode = DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode<DSAExperimentation.DataStructures.HashMap.HashMap<string, long>>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Parse Lisp Expression (LC 736): a baseline that gives every nested "let" its
// own full copy of the enclosing Dictionary<string,long> scope (an
// easy-to-reach-for interpreter design, but O(scope size) extra work per "let"
// on top of evaluating it) vs. this repo's own SinglyLinkedListNode<T> chaining
// per-"let" HashMap<string,long> scopes together (O(1) extra per "let", lookup
// walks Next), the same shape ParseLispExpressionTests uses. _expression
// chains Length nested "(let vI I ...)" scopes so the environment strategy
// gets exercised at every depth. Allocated is the fairer column to read here
// rather than Mean: both variants pay the same O(depth^2) string-slicing cost
// this simple recursive-descent parser re-pays at every nesting level
// regardless of environment strategy (each level re-strips and re-tokenizes
// its own shrinking substring), so that cost dominates wall-clock time for
// both. ScopeChain's allocation total still comes in lower at every size
// because it alone avoids also duplicating every prior binding on top of it.
[MemoryDiagnoser]
public class ParseLispExpressionBenchmarks
{
    private const string AddOperator = "add";
    private const string MultOperator = "mult";
    private const string LetExpressionPrefix = "(let v";
    private const int SecondOperandTokenIndex = 2;
    private const int LetBindingStride = 2;
    private const int PenultimateIndexOffset = 2;

    [Params(50, 400)]
    public int Length;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() => _expression = BuildNestedLetExpression(Length);

    [Benchmark(Baseline = true)]
    public long CopyEnvironmentPerLet() => EvaluateWithCopiedScope(_expression, new Dictionary<string, long>());

    [Benchmark]
    public long ScopeChain() => Evaluate(_expression, null);

    private static long EvaluateWithCopiedScope(string expr, Dictionary<string, long> scope)
    {
        if (expr[0] != '(')
        {
            return long.TryParse(expr, out var number) ? number : scope[expr];
        }

        var tokens = SplitTopLevelTokens(expr[1..^1]);

        return tokens[0] switch
        {
            AddOperator => EvaluateWithCopiedScope(tokens[1], scope) + EvaluateWithCopiedScope(tokens[SecondOperandTokenIndex], scope),
            MultOperator => EvaluateWithCopiedScope(tokens[1], scope) * EvaluateWithCopiedScope(tokens[SecondOperandTokenIndex], scope),
            _ => EvaluateLetWithCopiedScope(tokens, scope),
        };
    }

    private static long EvaluateLetWithCopiedScope(List<string> tokens, Dictionary<string, long> parentScope)
    {
        var scope = new Dictionary<string, long>(parentScope);

        for (var i = 1; i < tokens.Count - 1; i += LetBindingStride)
        {
            scope[tokens[i]] = EvaluateWithCopiedScope(tokens[i + 1], scope);
        }

        return EvaluateWithCopiedScope(tokens[^1], scope);
    }

    // See ParseLispExpressionTests for the full explanation - repeated here
    // rather than shared because TwoSumBenchmarks/
    // NumberOfLongestIncreasingSubsequenceBenchmarks establish this project
    // keeps its own copy of the solution rather than depending on the Tests
    // project.
    private static long Evaluate(string expr, RepoScopeNode? scope)
    {
        if (expr[0] != '(')
        {
            return ResolveAtom(expr, scope);
        }

        var tokens = SplitTopLevelTokens(expr[1..^1]);

        return tokens[0] switch
        {
            AddOperator => Evaluate(tokens[1], scope) + Evaluate(tokens[SecondOperandTokenIndex], scope),
            MultOperator => Evaluate(tokens[1], scope) * Evaluate(tokens[SecondOperandTokenIndex], scope),
            _ => EvaluateLet(tokens, scope),
        };
    }

    private static long EvaluateLet(List<string> tokens, RepoScopeNode? parentScope)
    {
        var bindings = new HashMap<string, long>();
        var letScope = new RepoScopeNode(bindings) { Next = parentScope };

        for (var i = 1; i < tokens.Count - 1; i += LetBindingStride)
        {
            var boundValue = Evaluate(tokens[i + 1], letScope);
            bindings.Set(tokens[i], boundValue);
        }

        return Evaluate(tokens[^1], letScope);
    }

    private static long ResolveAtom(string token, RepoScopeNode? scope)
    {
        if (long.TryParse(token, out var number))
        {
            return number;
        }

        for (var node = scope; node is not null; node = node.Next)
        {
            if (node.Value.TryGetValue(token, out var value))
            {
                return value;
            }
        }

        throw new InvalidOperationException($"Unbound variable '{token}'.");
    }

    private static List<string> SplitTopLevelTokens(string expr)
    {
        var tokens = new List<string>();
        var depth = 0;
        var start = 0;

        ScanTopLevelTokens(expr, tokens, ref depth, ref start);

        tokens.Add(expr[start..]);
        return tokens;
    }

    private static void ScanTopLevelTokens(string expr, List<string> tokens, ref int depth, ref int start)
    {
        for (var i = 0; i < expr.Length; i++)
        {
            switch (expr[i])
            {
                case '(':
                    depth++;
                    break;
                case ')':
                    depth--;
                    break;
                case ' ' when depth == 0:
                    tokens.Add(expr[start..i]);
                    start = i + 1;
                    break;
            }
        }
    }

    // Builds "(let v0 0 (let v1 1 (let v2 2 ... (add v0 (add v1 v2))...)))" -
    // Length nested lets, each shadowing nothing, wrapping a right-nested sum of
    // every bound variable.
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
