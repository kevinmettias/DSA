using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Whether a method's parameter list can take a call's arguments, judged from text: the argument
// count against its required and optional parameters, and each argument's declared type, where
// the caller's code states one, against the parameter's. A parameter type the text cannot judge -
// an interface, a type parameter, object, a tuple - fits anything, so only a clear mismatch rules
// an overload out.
internal static class OverloadFit
{
    private static readonly Regex Identifier = new(@"^\w+$");
    private static readonly Regex Construction = new(@"^new\s+(?<type>[\w.]+)\s*[<(]");
    private static readonly Regex TypeArguments = new("<[^<>]*>");
    private static readonly Regex Whitespace = new(@"\s+");
    private static readonly Regex TypeParameter = new("^T(?:[A-Z]\\w*)?$");
    private static readonly Regex InterfaceName = new("^I[A-Z]\\w*$");
    private static readonly HashSet<string> ParameterModifiers = ["this", "ref", "out", "in", "params", "scoped", "readonly"];
    private static readonly HashSet<string> Numeric = ["int", "long", "double", "float", "short", "byte", "decimal", "uint", "ulong"];

    public static bool CanTake(SourceDeclaration method, IReadOnlyList<string?> argumentTypes)
    {
        var parameters = ParametersOf(method.Signature);

        if (!CanTakeCount(parameters, argumentTypes.Count))
        {
            return false;
        }

        for (var index = 0; index < argumentTypes.Count && index < parameters.Count; index++)
        {
            if (!parameters[index].IsParams && !IsCompatible(argumentTypes[index], parameters[index].Type))
            {
                return false;
            }
        }

        return true;
    }

    // Each argument's type as far as the text states it - a field or property of the caller's
    // class, or the type a `new` expression constructs - and null where it does not.
    public static List<string?> ArgumentTypesOf(string arguments, IReadOnlyDictionary<string, string> fieldTypes)
    {
        var types = new List<string?>();

        foreach (var argument in SplitTopLevel(arguments))
        {
            types.Add(TypeOf(argument, fieldTypes));
        }

        return types;
    }

    // A type's last dotted segment with its type arguments and nullable mark removed, so
    // `WordLadderIISolution.BeginWord`, `List<int>` and `int[]?` compare as `BeginWord`, `List`
    // and `int[]`.
    public static string SimpleTypeName(string type)
    {
        var name = Whitespace.Replace(type, string.Empty).TrimEnd('?');

        while (TypeArguments.IsMatch(name))
        {
            name = TypeArguments.Replace(name, string.Empty);
        }

        var lastDot = name.LastIndexOf('.');

        return name[(lastDot + 1)..];
    }

    // A comma-separated list split at its own commas, not those inside brackets.
    public static List<string> SplitTopLevel(string list)
    {
        var parts = new List<string>();
        var start = 0;

        foreach (var comma in TopLevelCommas(list))
        {
            parts.Add(list[start..comma].Trim());
            start = comma + 1;
        }

        var last = list[start..].Trim();

        if (last.Length > 0)
        {
            parts.Add(last);
        }

        return parts;
    }

    private static IEnumerable<int> TopLevelCommas(string list)
    {
        var depth = 0;

        for (var index = 0; index < list.Length; index++)
        {
            depth += BracketChangeAt(list, index);

            if (depth == 0 && list[index] == ',')
            {
                yield return index;
            }
        }
    }

    // A signature's parameter list, parentheses included, or nothing when it has none.
    public static string ParameterListOf(string signature)
    {
        var close = signature.LastIndexOf(')');

        if (close < 0)
        {
            return string.Empty;
        }

        var open = MatchingOpen(signature, close);

        return signature[open..(close + 1)];
    }

    private static List<Parameter> ParametersOf(string signature)
    {
        var parameterList = ParameterListOf(signature);

        if (parameterList.Length == 0)
        {
            return [];
        }

        var inside = parameterList[1..^1];

        return [.. SplitTopLevel(inside).Select(ParameterOf).OfType<Parameter>()];
    }

    // The bracket that opens the `)` at `close`, scanning back.
    private static int MatchingOpen(string text, int close)
    {
        var depth = 0;

        for (var index = close; index >= 0; index--)
        {
            depth -= BracketChangeAt(text, index);

            if (depth == 0)
            {
                return index;
            }
        }

        return 0;
    }

    private static Parameter? ParameterOf(string declared)
    {
        var withoutAttribute = Regex.Replace(declared, @"^\[[^\]]*\]\s*", string.Empty);
        var hasDefault = withoutAttribute.Contains('=');
        var beforeDefault = withoutAttribute.Split('=')[0];
        var words = beforeDefault.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var isParams = words.Contains("params");
        var typeWords = words.Where(word => !ParameterModifiers.Contains(word)).ToList();

        if (typeWords.Count < 2)
        {
            return null;
        }

        var type = string.Join(' ', typeWords[..^1]);

        return new Parameter(SimpleTypeName(type), hasDefault, isParams);
    }

    private static bool CanTakeCount(List<Parameter> parameters, int argumentCount)
    {
        var required = parameters.Count(parameter => !parameter.HasDefault && !parameter.IsParams);

        if (parameters.Exists(parameter => parameter.IsParams))
        {
            return argumentCount >= required;
        }

        return required <= argumentCount && argumentCount <= parameters.Count;
    }

    // An argument whose type the text never stated, or a parameter whose type it cannot judge,
    // fits; otherwise the two must name the same type or both be numbers.
    private static bool IsCompatible(string? argumentType, string parameterType)
    {
        if (argumentType is null || IsUnjudgeable(parameterType))
        {
            return true;
        }

        return argumentType == parameterType || (Numeric.Contains(argumentType) && Numeric.Contains(parameterType));
    }

    private static bool IsUnjudgeable(string parameterType) =>
        parameterType == "object"
            || parameterType.StartsWith('(')
            || TypeParameter.IsMatch(parameterType)
            || InterfaceName.IsMatch(parameterType);

    private static string? TypeOf(string argument, IReadOnlyDictionary<string, string> fieldTypes)
    {
        var bare = Regex.Replace(argument, @"^(?:ref|out|in)\s+|^this\.", string.Empty);

        if (Identifier.IsMatch(bare))
        {
            return fieldTypes.GetValueOrDefault(bare);
        }

        var constructed = Construction.Match(bare);

        if (constructed.Success)
        {
            return SimpleTypeName(constructed.Groups["type"].Value);
        }

        return null;
    }

    // How the bracket at `index` changes the nesting depth, with a lambda's `=>` no bracket at all.
    private static int BracketChangeAt(string text, int index)
    {
        if (IsLambdaArrow(text, index))
        {
            return 0;
        }

        return text[index] switch
        {
            '(' or '<' or '[' or '{' => 1,
            ')' or '>' or ']' or '}' => -1,
            _ => 0,
        };
    }

    private static bool IsLambdaArrow(string text, int index) =>
        text[index] == '>' && index > 0 && text[index - 1] == '=';

    private readonly record struct Parameter(string Type, bool HasDefault, bool IsParams);
}
