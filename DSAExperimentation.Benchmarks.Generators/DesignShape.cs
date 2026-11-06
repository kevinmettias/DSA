using System.Text.RegularExpressions;

namespace DSAExperimentation.Benchmarks.Generators;

// What a design question's snapshot file says its class takes and answers (ARCHITECTURE 17.13): how many
// inputs its constructor takes, and each method's name as LeetCode's call scripts spell it, with how many
// inputs it takes. It is read from the file's text, as LeetCodeShape reads a function's declared types,
// since a generator cannot load the catalog. A declared type may hold brackets ("integer[]"), so each
// pattern steps over a quoted string whole.
internal sealed class DesignShape(int constructorInputs, IReadOnlyList<(string Name, int Inputs)> methods)
{
    // A quoted string, stepped over whole.
    private const string Quoted = "\"[^\"]*\"";

    // An array's items when none of them is an array: quoted strings, and anything but brackets and quotes.
    private const string FlatItems = $"(?:{Quoted}|[^\\]\\[\"])*";

    // The groups the patterns capture: an array's items, and a method's name and parameters.
    private const int ItemsGroup = 1;
    private const int NameGroup = 1;
    private const int ParametersGroup = 2;
    private const int TypeGroup = 2;
    private const string ArraySuffix = "[]";
    private const string Integer = "integer";

    private static readonly Regex NamedClass = new("\"ClassName\"\\s*:\\s*\"", RegexOptions.Compiled);

    private static readonly Regex ConstructorParameters =
        new($"\"ConstructorParameters\"\\s*:\\s*\\[({FlatItems})\\]", RegexOptions.Compiled);

    // The methods' array, whose items each hold one array of their own, the method's parameters.
    private static readonly Regex MethodsArray =
        new($"\"Methods\"\\s*:\\s*\\[((?:{Quoted}|\\[{FlatItems}\\]|[^\\]\\[\"])*)\\]", RegexOptions.Compiled);

    private static readonly Regex Method =
        new($"\"Name\"\\s*:\\s*\"([^\"]*)\"\\s*,\\s*\"Parameters\"\\s*:\\s*\\[({FlatItems})\\]", RegexOptions.Compiled);

    private static readonly Regex NamedParameter =
        new("\"Name\"\\s*:\\s*\"([^\"]*)\"\\s*,\\s*\"Type\"\\s*:\\s*\"([^\"]*)\"", RegexOptions.Compiled);

    // C's lengths of an array input, which the metaData lists and no call script passes, as the catalog's
    // LengthCompanions drops them: an integer named for an array listed before it.
    private static readonly string[] LengthSuffixes = ["Size", "RowSize", "ColSize"];

    public int ConstructorInputs { get; } = constructorInputs;

    // Each method LeetCode's scripts call, in the order the signature lists them.
    public IReadOnlyList<(string Name, int Inputs)> Methods { get; } = methods;

    // The design signature in a question's snapshot file; null for a function question.
    public static DesignShape? In(string questionJson)
    {
        if (!NamedClass.IsMatch(questionJson))
        {
            return null;
        }

        var constructor = ConstructorParameters.Match(questionJson).Groups[ItemsGroup].Value;
        var methodsArray = MethodsArray.Match(questionJson).Groups[ItemsGroup].Value;
        var methods = Method.Matches(methodsArray).Cast<Match>().Select(MethodOf).ToList();
        var constructorInputs = InputsIn(constructor);

        return new DesignShape(constructorInputs, methods);
    }

    private static (string Name, int Inputs) MethodOf(Match method)
    {
        var inputs = InputsIn(method.Groups[ParametersGroup].Value);

        return (method.Groups[NameGroup].Value, inputs);
    }

    // How many parameters a parameter array declares that a call script passes.
    private static int InputsIn(string parameters)
    {
        var declared = NamedParameter.Matches(parameters).Cast<Match>().Select(match => (Name: match.Groups[NameGroup].Value, Type: match.Groups[TypeGroup].Value)).ToList();
        var arrays = declared.Where(parameter => parameter.Type.EndsWith(ArraySuffix, StringComparison.Ordinal)).Select(parameter => parameter.Name).ToList();

        return declared.Count(parameter => !IsLengthCompanion(arrays, parameter));
    }

    private static bool IsLengthCompanion(List<string> arrays, (string Name, string Type) parameter)
        => parameter.Type == Integer && arrays.Any(array => LengthSuffixes.Any(suffix => parameter.Name == array + suffix));
}
