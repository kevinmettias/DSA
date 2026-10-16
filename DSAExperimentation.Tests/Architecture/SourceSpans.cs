using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Where a declaration ends in code whose comments and string contents RepositoryFiles.CodeOf has
// blanked - so every bracket left is a real one - and which words the code spells.
internal static class SourceSpans
{
    private static readonly Regex Word = new(@"\b[A-Za-z_]\w*\b");
    private static readonly char[] BodyOpeners = ['{', '=', ';'];
    private static readonly char[] TypeBodyOpeners = ['{', ';'];

    public static HashSet<string> WordsIn(string code) =>
        Word.Matches(code).Select(match => match.Value).ToHashSet(StringComparer.Ordinal);

    public static int CountOf(string name, string code)
    {
        var whole = $@"\b{Regex.Escape(name)}\b";

        return Regex.Count(code, whole);
    }

    // Just past the body of the method whose parameter list opens at `openParenthesis`: a block
    // body's closing brace, an expression body's semicolon, or a bodiless declaration's own.
    public static int BodyEnd(string code, int openParenthesis)
    {
        var afterParameters = MatchingClose(code, openParenthesis) + 1;
        var opener = code.IndexOfAny(BodyOpeners, afterParameters);

        if (opener < 0)
        {
            return code.Length;
        }

        return code[opener] switch
        {
            '{' => MatchingClose(code, opener) + 1,
            ';' => opener + 1,
            _ => StatementEnd(code, opener),
        };
    }

    // Just past the type declared up to `afterName`, and its primary-constructor parameter list
    // (empty without one): past the closing brace of its body, or the semicolon of a bodiless
    // record.
    public static (int End, string PrimaryParameters) TypeEnd(string code, int afterName)
    {
        var index = PastTypeParameters(code, afterName);
        var primaryParameters = string.Empty;

        if (index < code.Length && code[index] == '(')
        {
            var close = MatchingClose(code, index);
            primaryParameters = code[index..(close + 1)];
            index = close + 1;
        }

        var opener = code.IndexOfAny(TypeBodyOpeners, index);

        if (opener < 0)
        {
            return (code.Length, primaryParameters);
        }

        return (BlockEnd(code, opener), primaryParameters);
    }

    // Just past a field or property whose name ends at `afterName`: an accessor block and any
    // initializer after it, or an initializer or expression body up to its semicolon.
    public static int MemberEnd(string code, int afterName)
    {
        var index = PastWhitespace(code, afterName);

        if (index >= code.Length || code[index] != '{')
        {
            return StatementEnd(code, index);
        }

        var accessorsEnd = MatchingClose(code, index) + 1;
        var next = PastWhitespace(code, accessorsEnd);

        if (next < code.Length && code[next] == '=')
        {
            return StatementEnd(code, next);
        }

        return accessorsEnd;
    }

    // Where a call's argument list opens, given the index just past its method name: past any
    // type arguments, however deeply they nest. -1 when no argument list follows.
    public static int ArgumentListOpen(string code, int afterName)
    {
        var index = PastTypeParameters(code, afterName);

        if (index < code.Length && code[index] == '(')
        {
            return index;
        }

        return -1;
    }

    // The bracket that closes the one at `open`, counting nesting of the same pair.
    public static int MatchingClose(string code, int open)
    {
        var opening = code[open];
        var closing = ClosingOf(opening);
        var depth = 0;

        for (var index = open; index < code.Length; index++)
        {
            depth += DepthChange(code[index], opening, closing);

            if (depth == 0)
            {
                return index;
            }
        }

        return code.Length - 1;
    }

    private static char ClosingOf(char opening) => opening switch
    {
        '(' => ')',
        '[' => ']',
        '<' => '>',
        _ => '}',
    };

    private static int DepthChange(char character, char opening, char closing)
    {
        if (character == opening)
        {
            return 1;
        }

        if (character == closing)
        {
            return -1;
        }

        return 0;
    }

    // Just past the first semicolon from `from` with every bracket opened since closed again - so
    // a lambda block inside an expression does not end it early.
    private static int StatementEnd(string code, int from)
    {
        var depth = 0;

        for (var index = from; index < code.Length; index++)
        {
            depth += BracketChange(code[index]);

            if (depth <= 0 && code[index] == ';')
            {
                return index + 1;
            }
        }

        return code.Length;
    }

    private static int BracketChange(char character) => character switch
    {
        '(' or '{' or '[' => 1,
        ')' or '}' or ']' => -1,
        _ => 0,
    };

    private static int PastTypeParameters(string code, int afterName)
    {
        if (afterName < code.Length && code[afterName] == '<')
        {
            var close = MatchingClose(code, afterName);

            return PastWhitespace(code, close + 1);
        }

        return PastWhitespace(code, afterName);
    }

    private static int PastWhitespace(string code, int index)
    {
        while (index < code.Length && char.IsWhiteSpace(code[index]))
        {
            index++;
        }

        return index;
    }

    // A body's closing brace, or a bodiless declaration's semicolon.
    private static int BlockEnd(string code, int opener)
    {
        if (code[opener] == ';')
        {
            return opener + 1;
        }

        return MatchingClose(code, opener) + 1;
    }
}
