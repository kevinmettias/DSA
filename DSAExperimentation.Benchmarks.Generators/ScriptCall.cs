using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// One of LeetCode's methods as a generated design arm calls it (ARCHITECTURE 17.13): the call's name as the
// scripts spell it, and the method answering it - the made instance's own, or a method of the question's
// Calls type, statedOn, which takes the instance before the call's arguments. From the method come the code
// naming it, the inputs it reads, the type of its answer (null when it returns nothing), and why no arm can
// make the call, if none can.
internal sealed class ScriptCall(string name, IMethodSymbol answerer, string? statedOn)
{
    // The local a generated arm holds the factory's instance in.
    private const string MadeVariable = "made";

    public string Name { get; } = name;

    // "made.Put" for the instance's own method, "global::...LRUCacheCalls.Get" for a Calls method.
    public string Method { get; } = MethodOf(answerer, statedOn);

    // The arguments before the call's own: the instance, for a Calls method.
    public IReadOnlyList<string> LeadingArguments { get; } = LeadingArgumentsOf(statedOn);

    public IReadOnlyList<string> Inputs { get; } = [.. InputsOf(answerer, statedOn).Select(input => NotationTypes.Render(input.Type))];

    public string? AnswerType { get; } = AnswerTypeOf(answerer);

    // Why no arm can make the call, or null when one can.
    public string? ReasonUnbound { get; } = ReasonUnboundOf(name, answerer, statedOn);

    private static string MethodOf(IMethodSymbol answerer, string? statedOn)
    {
        if (statedOn is null)
        {
            return $"{MadeVariable}.{answerer.Name}";
        }

        return $"{statedOn}.{answerer.Name}";
    }

    private static IReadOnlyList<string> LeadingArgumentsOf(string? statedOn)
    {
        if (statedOn is null)
        {
            return [];
        }

        return [MadeVariable];
    }

    private static string? AnswerTypeOf(IMethodSymbol answerer)
    {
        if (answerer.ReturnsVoid)
        {
            return null;
        }

        return NotationTypes.Render(answerer.ReturnType);
    }

    private static string? ReasonUnboundOf(string name, IMethodSymbol answerer, string? statedOn)
    {
        if (InputsOf(answerer, statedOn).Select(ReasonUnreadable).FirstOrDefault(reason => reason is not null) is { } unreadable)
        {
            return $"{name}: {unreadable}";
        }

        if (answerer.ReturnsByRef || answerer.ReturnType.IsRefLikeType)
        {
            return $"{name} answers by reference, and an arm stores each answer by value";
        }

        return null;
    }

    // The parameters a script's arguments fill: all of the instance's own method's, a Calls method's after
    // the instance.
    private static IEnumerable<IParameterSymbol> InputsOf(IMethodSymbol answerer, string? statedOn)
    {
        if (statedOn is null)
        {
            return answerer.Parameters;
        }

        return answerer.Parameters.Skip(1);
    }

    // Why a script's argument cannot be read into the parameter, or null when it can.
    public static string? ReasonUnreadable(IParameterSymbol parameter)
    {
        if (parameter.RefKind != RefKind.None)
        {
            return $"parameter {parameter.Name} is passed by reference";
        }

        if (!NotationTypes.IsReadable(parameter.Type))
        {
            return $"LeetCode's notation has no reading for parameter {parameter.Name}'s {parameter.Type.ToDisplayString()}";
        }

        return null;
    }
}
