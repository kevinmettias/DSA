namespace DSAExperimentation.LeetCode.Solutions.ParseLispExpression;

// LC 736's promise about its input: the expression is legal. It is an integer, a variable that an
// enclosing "let" has bound, "(add e1 e2)", "(mult e1 e2)", or "(let v1 e1 ... vn en expr)" with one
// pair or more, each v a variable that the pairs after it and expr can read, and its tokens are
// parted by single spaces. LeetCode's judge never poses an input that breaks it. Both strategies read
// an expression without checking it: a parenthesis left open leaves an empty token they index into,
// an expression standing where a variable must is bound as though it were a name, and a name no "let"
// binds is looked up and not found. Generated cases do not always keep the promise, so a harness that
// runs them asks this first. It checks the grammar alone and evaluates nothing.
internal static class ParseLispExpressionPromise
{
    private const string AddOperator = "add";
    private const string LetOperator = "let";
    private const string MultOperator = "mult";

    private const char Open = '(';
    private const char Close = ')';
    private const char Space = ' ';
    private const char Minus = '-';

    // "let", one variable, the expression bound to it, and the expression the "let" answers with.
    private const int MinimumLetTokenCount = 4;

    // A variable and the expression bound to it.
    private const int LetBindingStride = 2;

    public static bool IsKeptBy(string expression)
    {
        var bound = new HashSet<string>();

        return IsLegal(expression, bound);
    }

    // An expression that opens a parenthesis is a form; anything else is an atom: an integer, or a
    // variable some enclosing "let" has bound, which is the only way a name enters the bound set.
    private static bool IsLegal(string expression, IReadOnlySet<string> bound)
    {
        if (expression.StartsWith(Open))
        {
            return IsLegalForm(expression, bound);
        }

        return IsInteger(expression) || bound.Contains(expression);
    }

    // A form closes with its last character the parenthesis it opens with, and its first token
    // names "add", "mult" or "let".
    private static bool IsLegalForm(string expression, IReadOnlySet<string> bound)
    {
        if (!expression.EndsWith(Close))
        {
            return false;
        }

        var body = expression[1..^1];

        if (!IsBalanced(body))
        {
            return false;
        }

        var tokens = TopLevelTokens(body);

        return tokens[0] switch
        {
            AddOperator or MultOperator => IsLegalOperation(tokens, bound),
            LetOperator => IsLegalLet(tokens, bound),
            _ => false,
        };
    }

    // Read left to right, no parenthesis closes before it opens, and every one that opens closes.
    private static bool IsBalanced(string body)
    {
        var depth = 0;

        foreach (var character in body)
        {
            depth += character switch
            {
                Open => 1,
                Close => -1,
                _ => 0,
            };

            if (depth < 0)
            {
                return false;
            }
        }

        return depth == 0;
    }

    // Cuts a form's body at the spaces outside every nested parenthesis, so "(add 1 2)" stays one
    // token. A doubled, leading or trailing space leaves an empty token, which no rule accepts.
    private static List<string> TopLevelTokens(string body)
    {
        var tokens = new List<string>();
        var depth = 0;
        var start = 0;

        for (var index = 0; index < body.Length; index++)
        {
            switch (body[index])
            {
                case Open:
                    depth++;
                    break;
                case Close:
                    depth--;
                    break;
                case Space when depth == 0:
                    tokens.Add(body[start..index]);
                    start = index + 1;
                    break;
            }
        }

        tokens.Add(body[start..]);

        return tokens;
    }

    // "add" and "mult" take exactly two operands, each legal in the scope the form sits in.
    private static bool IsLegalOperation(List<string> tokens, IReadOnlySet<string> bound)
        => tokens is [_, var left, var right] && IsLegal(left, bound) && IsLegal(right, bound);

    // "let", then one pair or more of a variable and the expression bound to it, then the expression
    // the "let" answers with: an even count of tokens, four at least. Each pair's expression is read
    // in the scope the pairs before it have built, and its variable joins that scope only after it,
    // for the pairs that follow and the final expression to read.
    private static bool IsLegalLet(List<string> tokens, IReadOnlySet<string> bound)
    {
        var isPaired = tokens.Count >= MinimumLetTokenCount && tokens.Count % LetBindingStride == 0;

        if (!isPaired)
        {
            return false;
        }

        var scope = new HashSet<string>(bound);

        for (var index = 1; index < tokens.Count - 1; index += LetBindingStride)
        {
            var variable = tokens[index];

            if (!IsVariable(variable) || !IsLegal(tokens[index + 1], scope))
            {
                return false;
            }

            scope.Add(variable);
        }

        return IsLegal(tokens[^1], scope);
    }

    // A variable: a lowercase letter, then lowercase letters or digits, and none of the three names
    // the grammar keeps for itself.
    private static bool IsVariable(string token)
    {
        var first = token.FirstOrDefault();
        var isName = char.IsAsciiLetterLower(first)
            && token.All(character => char.IsAsciiLetterLower(character) || char.IsAsciiDigit(character));

        return isName && token is not (AddOperator or LetOperator or MultOperator);
    }

    // An integer: an optional '-', then one digit or more.
    private static bool IsInteger(string token)
    {
        var digits = token is [Minus, .. var magnitude] ? magnitude : token;

        return digits.Length > 0 && digits.All(char.IsAsciiDigit);
    }
}
