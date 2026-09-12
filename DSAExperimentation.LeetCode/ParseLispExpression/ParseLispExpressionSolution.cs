using DSAExperimentation.DataStructures.HashMap;
using RepoScopeNode = DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode<DSAExperimentation.DataStructures.HashMap.HashMap<string, long>>;

namespace DSAExperimentation.LeetCode.ParseLispExpression;

// LeetCode 736. Parse Lisp Expression: recursive-descent evaluation of "let"/
// "add"/"mult" expressions. Both strategies parse identically - the only
// difference is how a nested "let" extends the environment its body sees.
internal static class ParseLispExpressionSolution
{
    private const string AddOperator = "add";
    private const string MultOperator = "mult";
    private const int SecondOperandTokenIndex = 2;
    private const int LetBindingStride = 2;

    // The naive baseline: every nested "let" gets its own full copy of the
    // enclosing scope (an easy-to-reach-for interpreter design), paying
    // O(scope size) extra work per "let" on top of evaluating it. Written
    // without this repo's own primitives, as a baseline is meant to be.
    public static long EvaluateByCopiedScope(string expression) =>
        EvaluateByCopiedScope(expression, new Dictionary<string, long>());

    private static long EvaluateByCopiedScope(string expr, Dictionary<string, long> scope)
    {
        if (expr[0] != '(')
        {
            return long.TryParse(expr, out var number) ? number : scope[expr];
        }

        var tokens = SplitTopLevelTokens(expr[1..^1]);

        return tokens[0] switch
        {
            AddOperator => EvaluateByCopiedScope(tokens[1], scope) + EvaluateByCopiedScope(tokens[SecondOperandTokenIndex], scope),
            MultOperator => EvaluateByCopiedScope(tokens[1], scope) * EvaluateByCopiedScope(tokens[SecondOperandTokenIndex], scope),
            _ => EvaluateLetByCopiedScope(tokens, scope),
        };
    }

    private static long EvaluateLetByCopiedScope(List<string> tokens, Dictionary<string, long> parentScope)
    {
        var scope = new Dictionary<string, long>(parentScope);

        for (var i = 1; i < tokens.Count - 1; i += LetBindingStride)
        {
            scope[tokens[i]] = EvaluateByCopiedScope(tokens[i + 1], scope);
        }

        return EvaluateByCopiedScope(tokens[^1], scope);
    }

    // This repo's own HashMap<string,long>, chained one per "let" via this
    // repo's own SinglyLinkedListNode<T>.Value/Next doubling as an
    // environment-chain link: O(1) extra per "let" rather than copying the
    // whole enclosing scope. Variable lookup walks Next from the innermost
    // scope outward until a HashMap.TryGetValue hits, which is exactly how
    // "let" shadowing is supposed to resolve - an inner "let x ..." hides an
    // outer x for the rest of its own body without mutating it.
    public static long EvaluateByScopeChain(string expression) => Evaluate(expression, null);

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

    // Splits an already-outer-paren-stripped expression on spaces at paren
    // depth 0 only, so a nested "(add x y)" token stays intact instead of
    // being cut on its own inner space. Shared by both strategies - it is
    // the same tokenizing step regardless of environment representation.
    private static List<string> SplitTopLevelTokens(string expr)
    {
        var tokens = new List<string>();
        var depth = 0;
        var start = 0;

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

        tokens.Add(expr[start..]);
        return tokens;
    }
}
