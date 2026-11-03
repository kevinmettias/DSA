using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// One strategy of an enrolled solution as a generated arm calls it: the inputs it reads from a case, in
// declaration order, and the answer it gives back - its return value alone, or the return value and its
// out parameters together as one tuple, as TwoSumSolution's TryFind arms answer (found, first, second).
// A strategy that cannot be called this way is refused with the reason, which the generator reports.
internal sealed class StrategyShape
{
    private const string Generic = "it is generic, so no one instantiation is the strategy";
    private const string NoStoredValue = "it answers in place or by reference, and an arm stores each case's answer by value";
    private const string TooManyOutputs = "its return value and out parameters are more than one tuple holds";
    private const string ItemSeparator = ", ";

    public string Name { get; }

    // Every parameter in declaration order, each written as C# and marked when it is an out parameter.
    public IReadOnlyList<(string Render, bool IsOutput)> Parameters { get; }

    // The parameters a case fills, in order: the types CatalogColumns reads its columns as.
    public IReadOnlyList<string> Inputs { get; }

    public int OutputCount { get; }

    public string AnswerType { get; }

    private StrategyShape(IMethodSymbol method)
    {
        Name = method.Name;
        Parameters = [.. method.Parameters.Select(parameter => (Render: NotationTypes.Render(parameter.Type), parameter.RefKind == RefKind.Out))];
        Inputs = [.. Parameters.Where(parameter => !parameter.IsOutput).Select(parameter => parameter.Render)];
        var outputs = Parameters.Where(parameter => parameter.IsOutput).Select(parameter => parameter.Render).ToList();
        var returned = NotationTypes.Render(method.ReturnType);
        OutputCount = outputs.Count;
        AnswerType = AnswerTypeOf(returned, outputs);
    }

    public static StrategyShape Of(IMethodSymbol method) => new(method);

    private static string AnswerTypeOf(string returned, List<string> outputs)
    {
        if (outputs.Count == 0)
        {
            return returned;
        }

        var items = new[] { returned }.Concat(outputs);

        return $"({string.Join(ItemSeparator, items)})";
    }

    // Why the method cannot be a generated arm, or null when it can.
    public static string? ReasonUnbound(IMethodSymbol method)
    {
        if (method.IsGenericMethod)
        {
            return Generic;
        }

        if (HasNoAnswerToStore(method))
        {
            return NoStoredValue;
        }

        if (method.Parameters.Count(parameter => parameter.RefKind == RefKind.Out) >= NotationTypes.MostTupleItems)
        {
            return TooManyOutputs;
        }

        return method.Parameters.Select(ReasonUnbound).FirstOrDefault(reason => reason is not null);
    }

    // A void method answers by mutating its input, and a ref return or a ref struct cannot sit in an array.
    private static bool HasNoAnswerToStore(IMethodSymbol method) =>
        method.ReturnsVoid || method.ReturnsByRef || method.ReturnType.IsRefLikeType;

    private static string? ReasonUnbound(IParameterSymbol parameter) => parameter.RefKind switch
    {
        RefKind.Out when parameter.Type.IsRefLikeType => $"out parameter {parameter.Name} is a ref struct",
        RefKind.Out => null,
        RefKind.None when NotationTypes.IsReadable(parameter.Type) => null,
        RefKind.None => $"LeetCode's notation has no reading for parameter {parameter.Name}'s {parameter.Type.ToDisplayString()}",
        _ => $"parameter {parameter.Name} is passed by reference",
    };
}
