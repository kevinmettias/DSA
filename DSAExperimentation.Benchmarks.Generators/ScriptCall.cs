using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// One of LeetCode's methods as a generated design arm calls it (ARCHITECTURE 17.13): the call's name as the
// scripts spell it, the method answering it as generated code names it, the arguments that come before the
// call's own (the object, for a method of the question's Calls type), the inputs the method reads, and the
// type of its answer - null when it returns nothing.
internal sealed class ScriptCall
{
    private ScriptCall(string name, string method, IReadOnlyList<string> leadingArguments, IReadOnlyList<string> inputs, string? answerType)
    {
        Name = name;
        Method = method;
        LeadingArguments = leadingArguments;
        Inputs = inputs;
        AnswerType = answerType;
    }

    public string Name { get; }

    // "made.Put" for the object's own method, "global::...LRUCacheCalls.Get" for a Calls method.
    public string Method { get; }

    public IReadOnlyList<string> LeadingArguments { get; }

    public IReadOnlyList<string> Inputs { get; }

    public string? AnswerType { get; }

    public static ScriptCall Of(string name, string method, IReadOnlyList<string> leadingArguments, IReadOnlyList<IParameterSymbol> inputs, IMethodSymbol answerer) =>
        new(name, method, leadingArguments, [.. inputs.Select(input => NotationTypes.Render(input.Type))], AnswerTypeOf(answerer));

    private static string? AnswerTypeOf(IMethodSymbol answerer)
    {
        if (answerer.ReturnsVoid)
        {
            return null;
        }

        return NotationTypes.Render(answerer.ReturnType);
    }
}
