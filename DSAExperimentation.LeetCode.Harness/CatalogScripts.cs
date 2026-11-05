using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Harness;

// A design question's scripts laid out for a generated arm (ARCHITECTURE 17.13), read once in [GlobalSetup]:
// which of LeetCode's methods each call makes, the constructor's inputs per case, and each method's
// arguments per case and call, read into the types the method answering it takes. The arm then indexes
// plain arrays and makes each call directly, as a function question's arm reads CatalogColumns, and
// writes each answer into a buffer of its method's return type.
internal sealed class CatalogScripts
{
    // What Calls holds for a script's first call, the constructor's, before any of LeetCode's methods.
    public const int Constructor = -1;

    private readonly JsonArray[] _arguments;

    public CatalogScripts(CatalogColumns columns, IReadOnlyList<string> methods)
    {
        _arguments = columns.Column<JsonArray>(1);
        Calls = [.. columns.Column<string[]>(0).Select(script => IndicesOf(script, methods))];
    }

    // For each case, the index in LeetCode's method list of each call's method, Constructor for the first.
    public int[][] Calls { get; }

    // Every case's constructor input at one position, as the factory takes it.
    public T[] ConstructorColumn<T>(int parameter) => [.. _arguments.Select(arguments => ValueOf<T>(arguments[0], parameter))];

    // For each case and call, the call's argument at one position when the call is to this method, and the
    // type's default for every other call.
    public T[][] ArgumentColumn<T>(int method, int parameter) =>
        [.. _arguments.Select((arguments, index) => ArgumentsOf<T>(arguments, Calls[index], method, parameter))];

    // A buffer for one method's answers: for each case, a slot per call.
    public T[][] AnswerBuffer<T>() => [.. Calls.Select(script => new T[script.Length])];

    // An arm's answers, read back from the buffer of each method that returns a value, by method index, with
    // null for each method that returns nothing.
    public CatalogCallAnswers AnswersFrom(params Array?[] buffersByMethod) => new(Calls, buffersByMethod);

    private static int[] IndicesOf(string[] script, IReadOnlyList<string> methods)
    {
        var indices = new int[script.Length];
        indices[0] = Constructor;

        for (var call = 1; call < script.Length; call++)
        {
            indices[call] = IndexOf(methods, script[call]);
        }

        return indices;
    }

    private static int IndexOf(IReadOnlyList<string> methods, string called)
    {
        for (var method = 0; method < methods.Count; method++)
        {
            if (methods[method] == called)
            {
                return method;
            }
        }

        throw new InvalidOperationException($"A script calls {called}, which the question's methods do not name.");
    }

    private static T[] ArgumentsOf<T>(JsonArray arguments, int[] script, int method, int parameter)
    {
        var values = new T[script.Length];

        for (var call = 0; call < script.Length; call++)
        {
            if (script[call] == method)
            {
                values[call] = ValueOf<T>(arguments[call], parameter);
            }
        }

        return values;
    }

    private static T ValueOf<T>(JsonNode? arguments, int parameter)
    {
        var text = CatalogCalls.ArgumentTextOf(arguments, parameter);

        return (T)NotationReader.Read(text, typeof(T))!;
    }
}
