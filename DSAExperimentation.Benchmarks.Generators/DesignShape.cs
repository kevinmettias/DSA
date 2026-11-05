using System.Text.RegularExpressions;

namespace DSAExperimentation.Benchmarks.Generators;

// What a design question's snapshot file says its class takes and answers (ARCHITECTURE 17.13): how many
// inputs its constructor takes, and each method's name as LeetCode's call scripts spell it, with how many
// inputs it takes. It is read from the file's text, as LeetCodeShape reads a function's declared types,
// since a generator cannot load the catalog.
internal sealed class DesignShape
{
    private const string ClassNameKey = "\"ClassName\"";
    private const string ConstructorKey = "\"ConstructorParameters\"";
    private const string MethodsKey = "\"Methods\"";
    private const char Quote = '"';
    private const char Escape = '\\';
    private const char ArrayOpen = '[';
    private const char ObjectOpen = '{';
    private const char ArrayClose = ']';
    private const char ObjectClose = '}';

    // The depth of an object directly inside the array a scan starts at.
    private const int ElementDepth = 2;

    private static readonly Regex NamedClass = new("\"ClassName\"\\s*:\\s*\"", RegexOptions.Compiled);
    private static readonly Regex MethodName = new("\"Name\"\\s*:\\s*\"([^\"]*)\"", RegexOptions.Compiled);
    private static readonly Regex DeclaredType = new("\"Type\"\\s*:", RegexOptions.Compiled);

    private DesignShape(int constructorInputs, IReadOnlyList<(string Name, int Inputs)> methods)
    {
        ConstructorInputs = constructorInputs;
        Methods = methods;
    }

    public int ConstructorInputs { get; }

    // Each method LeetCode's scripts call, in the order the signature lists them.
    public IReadOnlyList<(string Name, int Inputs)> Methods { get; }

    // The design signature in a question's snapshot file; null for a function question.
    public static DesignShape? In(string questionJson)
    {
        if (!NamedClass.IsMatch(questionJson))
        {
            return null;
        }

        var className = questionJson.IndexOf(ClassNameKey, StringComparison.Ordinal);

        var constructor = ObjectsIn(questionJson, questionJson.IndexOf(ArrayOpen, questionJson.IndexOf(ConstructorKey, className, StringComparison.Ordinal)));
        var methods = ObjectsIn(questionJson, questionJson.IndexOf(ArrayOpen, questionJson.IndexOf(MethodsKey, className, StringComparison.Ordinal)));

        return new DesignShape(constructor.Count, [.. methods.Select(MethodOf)]);
    }

    private static (string Name, int Inputs) MethodOf(string method) =>
        (MethodName.Match(method).Groups[1].Value, DeclaredType.Matches(method).Count);

    // The objects directly inside the array whose bracket is at open, each as its text; brackets inside
    // strings, such as a declared type's "integer[]", are skipped.
    private static List<string> ObjectsIn(string json, int open)
    {
        var objects = new List<string>();
        var depth = 0;
        var start = 0;

        for (var index = open; index < json.Length && depth >= 0; index++)
        {
            var character = json[index];

            if (character == Quote)
            {
                index = EndOfString(json, index);
            }
            else if (character is ArrayOpen or ObjectOpen)
            {
                depth++;
                start = ElementStartAt(index, depth, start);
            }
            else if (character is ArrayClose or ObjectClose)
            {
                AddElementEndingAt(json, index, depth, start, objects);
                depth = NextDepthAfterClose(depth);
            }
        }

        return objects;
    }

    // Where an element starts: here when this bracket opens one, else where the open element started.
    private static int ElementStartAt(int index, int depth, int start)
    {
        if (depth != ElementDepth)
        {
            return start;
        }

        return index;
    }

    private static void AddElementEndingAt(string json, int index, int depth, int start, List<string> objects)
    {
        if (depth == ElementDepth)
        {
            objects.Add(json.Substring(start, index + 1 - start));
        }
    }

    // Closing the array the scan started at ends the scan.
    private static int NextDepthAfterClose(int depth)
    {
        if (depth == 1)
        {
            return -1;
        }

        return depth - 1;
    }

    // The index of the quote closing the string that opens at start.
    private static int EndOfString(string json, int start)
    {
        var index = start + 1;

        while (index < json.Length && json[index] != Quote)
        {
            index += json[index] == Escape ? 2 : 1;
        }

        return index;
    }
}
