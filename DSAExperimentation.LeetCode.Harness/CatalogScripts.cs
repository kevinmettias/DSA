using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Harness;

// A design question's scripts laid out for a generated arm (ARCHITECTURE 17.13), read once in [GlobalSetup]:
// which of LeetCode's methods each call makes, the constructor's inputs per case, and each method's
// arguments per case and call, read into the types the method answering it takes. The arm then indexes
// plain arrays and makes each call directly, as a function question's arm reads CatalogColumns, and
// writes each answer into a buffer of its method's return type.
internal sealed class CatalogScripts(CatalogColumns columns, IReadOnlyList<string> methods)
{
    // What Calls holds for a script's first call, the constructor's, before any of LeetCode's methods.
    public const int Constructor = -1;

    private readonly JsonArray[] _arguments = columns.Column<JsonArray>(1);

    // For each case, the index in LeetCode's method list of each call's method, Constructor for the first.
    public int[][] Calls { get; } = [.. columns.Column<string[]>(0).Select(script => IndicesOf(script, methods))];

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

    // Every case's constructor input at one position, as the factory takes it.
    public TInput?[] ConstructorColumn<TInput>(int parameter) => [.. _arguments.Select(arguments => ValueOf<TInput>(arguments[0], parameter))];

    // For each case and call, the call's argument at one position when the call is to this method, and the
    // type's default for every other call.
    public TInput?[][] ArgumentColumn<TInput>(int method, int parameter) =>
        [.. _arguments.Select((arguments, index) => ArgumentsOf<TInput>(arguments, Calls[index], method, parameter))];

    private static TInput?[] ArgumentsOf<TInput>(JsonArray arguments, int[] script, int method, int parameter)
    {
        var values = new TInput?[script.Length];

        for (var call = 0; call < script.Length; call++)
        {
            if (script[call] == method)
            {
                values[call] = ValueOf<TInput>(arguments[call], parameter);
            }
        }

        return values;
    }

    // An argument read into its parameter's type; null only where LeetCode lists null, which a reference type
    // such as a missing tree holds.
    private static TInput? ValueOf<TInput>(JsonNode? arguments, int parameter)
    {
        var text = CatalogCalls.ArgumentTextOf(arguments, parameter);

        return (TInput?)NotationReader.Read(text, typeof(TInput));
    }

    // A buffer for one method's answers: for each case, a slot per call.
    public TAnswer[][] AnswerBuffer<TAnswer>() => [.. Calls.Select(script => new TAnswer[script.Length])];

    // An arm's answers, read back from the buffer of each method that returns a value, by method index, with
    // null for each method that returns nothing. Not params: one buffer alone, a T[][], would convert to the
    // Array[] itself by array covariance and stand for every method's buffer.
    public CatalogCallAnswers AnswersFrom(Array?[] buffersByMethod) => new(Calls, buffersByMethod);
}
