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

    private static readonly Regex NamedClass = new("\"ClassName\"\\s*:\\s*\"", RegexOptions.Compiled);

    private static readonly Regex ConstructorParameters =
        new($"\"ConstructorParameters\"\\s*:\\s*\\[({FlatItems})\\]", RegexOptions.Compiled);

    // The methods' array, whose items each hold one array of their own, the method's parameters.
    private static readonly Regex MethodsArray =
        new($"\"Methods\"\\s*:\\s*\\[((?:{Quoted}|\\[{FlatItems}\\]|[^\\]\\[\"])*)\\]", RegexOptions.Compiled);

    private static readonly Regex Method =
        new($"\"Name\"\\s*:\\s*\"([^\"]*)\"\\s*,\\s*\"Parameters\"\\s*:\\s*\\[({FlatItems})\\]", RegexOptions.Compiled);

    private static readonly Regex DeclaredType = new("\"Type\"\\s*:", RegexOptions.Compiled);

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

    // How many parameters a parameter array declares, one type each.
    private static int InputsIn(string parameters) => DeclaredType.Matches(parameters).Count;
}
