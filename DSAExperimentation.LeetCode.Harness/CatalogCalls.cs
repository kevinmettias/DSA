using System.Reflection;
using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Harness;

// How one strategy of a design question answers LeetCode's call scripts (ARCHITECTURE 17.13): the strategy is
// a factory that takes the constructor's inputs and makes the instance the script calls, and each of the
// question's calls is bound to the method that answers it (CatalogCallBinder). A replay makes the instance,
// then each call in turn, reading each argument into the parameter that takes it.
internal sealed class CatalogCalls(MethodInfo factory, IReadOnlyDictionary<string, MethodInfo> answererByCall)
{
    // An argument LeetCode lists as null, as a missing tree child is written.
    private const string JsonNull = "null";

    private readonly MethodInfo _factory = factory;
    private readonly IReadOnlyDictionary<string, MethodInfo> _answererByCall = answererByCall;

    // The script's answer as LeetCode writes it: null for the constructor, then each call's return value,
    // or null for a call that returns nothing.
    public JsonArray AnswerTo(IReadOnlyList<string> calls, JsonArray arguments)
    {
        var constructorArguments = ArgumentsOf(_factory.GetParameters(), arguments[0]);
        var target = Invoke(_factory, null, constructorArguments);
        var answers = new JsonArray { null };

        for (var call = 1; call < calls.Count; call++)
        {
            var answer = AnswerOf(_answererByCall[calls[call]], target, arguments[call]);
            answers.Add(answer);
        }

        return answers;
    }

    private static JsonNode? AnswerOf(MethodInfo answerer, object? target, JsonNode? arguments)
    {
        var returned = CallOf(answerer, target, arguments);

        if (answerer.ReturnType == typeof(void))
        {
            return null;
        }

        return AnswerNotation.Of(returned);
    }

    // A Calls method takes the instance before the call's arguments; the instance's own method is called on it.
    private static object? CallOf(MethodInfo answerer, object? target, JsonNode? arguments)
    {
        var parameters = answerer.GetParameters();

        if (!answerer.IsStatic)
        {
            var own = ArgumentsOf(parameters, arguments);
            return Invoke(answerer, target, own);
        }

        object?[] withTarget = [target, .. ArgumentsOf(parameters[1..], arguments)];
        return Invoke(answerer, null, withTarget);
    }

    // One argument of a call, from the array LeetCode lists the call's arguments in, as notation to read.
    public static string ArgumentTextOf(JsonNode? arguments, int position) =>
        arguments?.AsArray()[position]?.ToJsonString() ?? JsonNull;

    private static object? Invoke(MethodInfo method, object? target, object?[] arguments) =>
        method.Invoke(target, BindingFlags.DoNotWrapExceptions, binder: null, arguments, culture: null);

    // A call's arguments, as LeetCode lists them, read into the parameters that take them.
    private static object?[] ArgumentsOf(ParameterInfo[] parameters, JsonNode? arguments) =>
        [.. parameters.Select((parameter, position) => ArgumentOf(parameter, arguments, position))];

    private static object? ArgumentOf(ParameterInfo parameter, JsonNode? arguments, int position)
    {
        var text = ArgumentTextOf(arguments, position);

        return NotationReader.Read(text, parameter.ParameterType);
    }
}
