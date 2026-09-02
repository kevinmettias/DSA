using DSAExperimentation.DataStructures.HashMap;
using RepoScopeNode = DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode<DSAExperimentation.DataStructures.HashMap.HashMap<string, long>>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParseLispExpression;

// LeetCode 736. Parse Lisp Expression: recursive-descent evaluation of "let"/
// "add"/"mult" expressions. Each "let" pushes a new scope onto a chain of this
// repo's own HashMap<string,long> instances, linked via this repo's own
// SinglyLinkedListNode<T>.Value/Next doubling as an environment-chain link -
// the same "compose an existing Representation type for a new purpose" move
// NumberOfLongestIncreasingSubsequenceTests already makes for ICombineOperation,
// just reusing a node type instead of an interface. Variable lookup walks Next
// from the innermost scope outward until a HashMap.TryGetValue hits, which is
// exactly how "let" shadowing is supposed to resolve - an inner "let x ..."
// hides an outer x for the rest of its own body without mutating it.
public sealed partial class ParseLispExpressionTests
{
    [Theory]
    [InlineData("(let x 2 (mult x (let x 3 y 4 (add x y))))", 14)]
    [InlineData("(let x 3 x 2 x)", 2)]
    [InlineData("(let x 1 y 2 x (add x y) (add x y))", 5)]
    [InlineData("(add 1 2)", 3)]
    [InlineData("(mult 3 (add 2 3))", 15)]
    public void Evaluate_LeetCodeExamples_ReturnsExpectedValue(string expression, long expected)
    {
        var actual = Evaluate(expression, null);
        Assert.Equal(expected, actual);
    }

    private static long Evaluate(string expr, RepoScopeNode? scope)
    {
        if (expr[0] != '(')
        {
            return ResolveAtom(expr, scope);
        }

        var tokens = SplitTopLevelTokens(expr[1..^1]);

        return tokens[0] switch
        {
            "add" => Evaluate(tokens[1], scope) + Evaluate(tokens[2], scope),
            "mult" => Evaluate(tokens[1], scope) * Evaluate(tokens[2], scope),
            _ => EvaluateLet(tokens, scope),
        };
    }

    private static long EvaluateLet(List<string> tokens, RepoScopeNode? parentScope)
    {
        var bindings = new HashMap<string, long>();
        var letScope = new RepoScopeNode(bindings) { Next = parentScope };

        for (var i = 1; i < tokens.Count - 1; i += 2)
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

    // Splits an already-outer-paren-stripped expression on spaces at paren depth
    // 0 only, so a nested "(add x y)" token stays intact instead of being cut on
    // its own inner space.
    private static List<string> SplitTopLevelTokens(string expr)
    {
        var splitPoints = FindTopLevelSpaces(expr);
        return ExtractTokensBetween(expr, splitPoints);
    }

    private static List<int> FindTopLevelSpaces(string expr)
    {
        var splitPoints = new List<int>();
        var depth = 0;

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
                    splitPoints.Add(i);
                    break;
            }
        }

        return splitPoints;
    }

    private static List<string> ExtractTokensBetween(string expr, List<int> splitPoints)
    {
        var tokens = new List<string>();
        var start = 0;

        foreach (var splitPoint in splitPoints)
        {
            tokens.Add(expr[start..splitPoint]);
            start = splitPoint + 1;
        }

        tokens.Add(expr[start..]);
        return tokens;
    }
}
