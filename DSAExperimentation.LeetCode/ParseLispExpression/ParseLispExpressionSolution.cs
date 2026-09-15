namespace DSAExperimentation.LeetCode.ParseLispExpression;

// LeetCode 736. Parse Lisp Expression: recursive-descent evaluation of "let"/
// "add"/"mult" expressions. Both strategies parse identically - the only
// difference is how a nested "let" extends the environment its body sees - so
// that difference is named rather than restated: both arms evaluate against an
// IScopeEnvironment and implement only the four decision points it declares.
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
        EvaluateExpression(expression, CopiedScopeEnvironment.Instance);

    // This repo's own HashMap<string,long>, chained one per "let" via this
    // repo's own SinglyLinkedListNode<T>.Value/Next doubling as an
    // environment-chain link: O(1) extra per "let" rather than copying the
    // whole enclosing scope.
    public static long EvaluateByScopeChain(string expression) =>
        EvaluateExpression(expression, ScopeChainEnvironment.Instance);

    // Both arms evaluate against the environment they are handed; each arm's own
    // entry point above is the only thing that picks one.
    private static long EvaluateExpression<TScope>(string expression, IScopeEnvironment<TScope> environment)
    {
        var scope = environment.Root();

        return Evaluate(expression, scope, environment);
    }

    private static long Evaluate<TScope>(string expr, TScope scope, IScopeEnvironment<TScope> environment)
    {
        if (expr[0] != '(')
        {
            return ResolveAtom(expr, scope, environment);
        }

        var tokens = SplitTopLevelTokens(expr[1..^1]);

        return tokens[0] switch
        {
            AddOperator => Evaluate(tokens[1], scope, environment) + Evaluate(tokens[SecondOperandTokenIndex], scope, environment),
            MultOperator => Evaluate(tokens[1], scope, environment) * Evaluate(tokens[SecondOperandTokenIndex], scope, environment),
            _ => EvaluateLet(tokens, scope, environment),
        };
    }

    // A "let" body: extend the scope, then bind each pair into it in order,
    // evaluating a bound expression against the bindings written before it -
    // which is what makes "(let x 1 y (add x 1))" legal - and answer with the
    // last token, the expression the "let" is there to evaluate.
    private static long EvaluateLet<TScope>(List<string> tokens, TScope parentScope, IScopeEnvironment<TScope> environment)
    {
        var scope = environment.Extend(parentScope);

        for (var i = 1; i < tokens.Count - 1; i += LetBindingStride)
        {
            var boundValue = Evaluate(tokens[i + 1], scope, environment);
            environment.Bind(scope, tokens[i], boundValue);
        }

        return Evaluate(tokens[^1], scope, environment);
    }

    // The atom test both arms share: a decimal literal evaluates the same under
    // either environment, so only a name is handed on to be resolved. Keeping it
    // here is what leaves each arm with one decision point - how a name is looked
    // up - instead of a second copy of the test.
    private static long ResolveAtom<TScope>(string token, TScope scope, IScopeEnvironment<TScope> environment)
    {
        if (long.TryParse(token, out var number))
        {
            return number;
        }

        return environment.ResolveName(token, scope);
    }

    // Splits an already-outer-paren-stripped expression on spaces at paren
    // depth 0 only, so a nested "(add x y)" token stays intact instead of
    // being cut on its own inner space. Shared by both strategies - it is
    // the same tokenizing step regardless of environment representation.
    private static List<string> SplitTopLevelTokens(string expr)
    {
        var tokens = new List<string>();
        AppendTopLevelTokens(expr, tokens);

        return tokens;
    }

    // The scan itself: walk every character, tracking paren depth so only the spaces
    // outside every nested "(...)" become cuts.
    private static void AppendTopLevelTokens(string expr, List<string> tokens)
    {
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
    }
}
