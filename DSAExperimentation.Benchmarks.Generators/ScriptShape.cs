using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// One strategy of an enrolled design solution as a generated arm calls it (ARCHITECTURE 17.13): a factory
// making the object from the constructor's inputs, and for each of LeetCode's methods the method answering
// it - the question's Calls type's static method of the call's name, ignoring case, which takes the object
// first, or else the object's own - with the inputs it reads and the answer it gives. The harness's
// CatalogCalls binds the same methods at run time. A strategy that cannot be called this way is refused with
// the reason, which the generator reports.
internal sealed class ScriptShape
{
    private const string Generic = "it is generic, so no one instantiation is the strategy";
    private const string NoObject = "it makes no object a script can call";
    // The local a generated arm holds the factory's object in.
    private const string MadeVariable = "made";

    // What separates the words of a few of LeetCode's method names, as in get_root.
    private const string WordBreak = "_";

    private ScriptShape(IMethodSymbol factory, IReadOnlyList<ScriptCall> calls)
    {
        Name = factory.Name;
        Inputs = [.. factory.Parameters.Select(parameter => NotationTypes.Render(parameter.Type))];
        Calls = calls;
    }

    public string Name { get; }

    // The constructor's inputs, as the factory takes them.
    public IReadOnlyList<string> Inputs { get; }

    // The method answering each of LeetCode's methods, in the order the question's signature lists them.
    public IReadOnlyList<ScriptCall> Calls { get; }

    // The arm a factory makes, or why it makes none.
    public static (ScriptShape? Shape, string? Reason) Of(IMethodSymbol factory, INamedTypeSymbol? stated, DesignShape design)
    {
        if (ReasonUnbound(factory, design) is { } reason)
        {
            return (null, reason);
        }

        var calls = new List<ScriptCall>();

        foreach (var (name, inputs) in design.Methods)
        {
            var (call, refusal) = CallOf(factory.ReturnType, stated, name, inputs);

            if (call is null)
            {
                return (null, refusal);
            }

            calls.Add(call);
        }

        return (new ScriptShape(factory, calls), null);
    }

    private static string? ReasonUnbound(IMethodSymbol factory, DesignShape design)
    {
        if (factory.IsGenericMethod)
        {
            return Generic;
        }

        if (factory.ReturnsVoid || factory.ReturnsByRef || factory.ReturnType.IsRefLikeType)
        {
            return NoObject;
        }

        if (factory.Parameters.Length != design.ConstructorInputs)
        {
            return $"it takes {factory.Parameters.Length} inputs, and LeetCode's constructor takes {design.ConstructorInputs}";
        }

        return factory.Parameters.Select(ReasonUnreadable).FirstOrDefault(unreadable => unreadable is not null);
    }

    // The call answering one of LeetCode's methods on the object, or why none does.
    private static (ScriptCall? Call, string? Reason) CallOf(ITypeSymbol made, INamedTypeSymbol? stated, string name, int inputs)
    {
        var answerer = StatedAnswererOf(made, stated, name, inputs);
        var isStated = answerer is not null;
        answerer ??= OwnAnswererOf(made, name, inputs);

        if (answerer is null)
        {
            return (null, $"its {made.Name} answers no {name} call of {inputs} inputs by one method, and the question's Calls type states none");
        }

        var parameters = answerer.Parameters.Skip(isStated ? 1 : 0).ToList();

        if (parameters.Select(ReasonUnreadable).FirstOrDefault(unreadable => unreadable is not null) is { } reason)
        {
            return (null, $"{name}: {reason}");
        }

        if (answerer.ReturnsByRef || answerer.ReturnType.IsRefLikeType)
        {
            return (null, $"{name} answers by reference, and an arm stores each answer by value");
        }

        return (CallTo(name, answerer, isStated, parameters), null);
    }

    // A Calls method is named on its type and takes the object first; the object's own is named on it.
    private static ScriptCall CallTo(string name, IMethodSymbol answerer, bool isStated, List<IParameterSymbol> parameters)
    {
        if (!isStated)
        {
            return ScriptCall.Of(name, $"{MadeVariable}.{answerer.Name}", [], parameters, answerer);
        }

        return ScriptCall.Of(name, $"{NotationTypes.Render(answerer.ContainingType)}.{answerer.Name}", [MadeVariable], parameters, answerer);
    }

    private static IMethodSymbol? StatedAnswererOf(ITypeSymbol made, INamedTypeSymbol? stated, string name, int inputs)
    {
        if (stated is null)
        {
            return null;
        }

        var candidates = stated.GetMembers().OfType<IMethodSymbol>()
            .Where(method => method is { IsStatic: true, MethodKind: MethodKind.Ordinary } && IsNamedFor(method, name) && TakesTheObjectAndInputs(method, made, inputs))
            .ToList();

        return TheOneOf(candidates);
    }

    private static IMethodSymbol? OwnAnswererOf(ITypeSymbol made, string name, int inputs)
    {
        var candidates = InstanceMethodsOf(made)
            .Where(method => IsNamedFor(method, name) && method.Parameters.Length == inputs)
            .ToList();

        return TheOneOf(candidates);
    }

    private static IMethodSymbol? TheOneOf(List<IMethodSymbol> candidates)
    {
        if (candidates.Count != 1)
        {
            return null;
        }

        return candidates[0];
    }

    // Named for the call ignoring case, and the underscores LeetCode writes in a few names (get_root).
    private static bool IsNamedFor(IMethodSymbol method, string name) =>
        string.Equals(method.Name, name.Replace(WordBreak, string.Empty), StringComparison.OrdinalIgnoreCase);

    private static bool TakesTheObjectAndInputs(IMethodSymbol method, ITypeSymbol made, int inputs) =>
        method.Parameters.Length == inputs + 1 && IsAssignableTo(made, method.Parameters[0].Type);

    // The object's type is the parameter's, or one of the classes or interfaces it extends.
    private static bool IsAssignableTo(ITypeSymbol made, ITypeSymbol target) =>
        SymbolEqualityComparer.Default.Equals(made, target)
            || made.AllInterfaces.Any(contract => SymbolEqualityComparer.Default.Equals(contract, target))
            || BaseTypesOf(made).Any(baseType => SymbolEqualityComparer.Default.Equals(baseType, target));

    // The public or internal instance methods of the object's type: an interface's with those of the
    // interfaces it extends, a class's with those it inherits, each override counted once, as the method it
    // overrides.
    private static IEnumerable<IMethodSymbol> InstanceMethodsOf(ITypeSymbol made) =>
        TypesDeclaringMethodsOf(made)
            .SelectMany(type => type.GetMembers().OfType<IMethodSymbol>())
            .Where(method => method is { IsStatic: false, IsOverride: false, MethodKind: MethodKind.Ordinary, DeclaredAccessibility: Accessibility.Public or Accessibility.Internal });

    // A class implements its interfaces' methods itself, so only an interface reads the ones it extends.
    private static IEnumerable<ITypeSymbol> TypesDeclaringMethodsOf(ITypeSymbol made)
    {
        if (made.TypeKind != TypeKind.Interface)
        {
            return BaseTypesOf(made).Prepend(made);
        }

        return made.AllInterfaces.Cast<ITypeSymbol>().Prepend(made);
    }

    private static IEnumerable<ITypeSymbol> BaseTypesOf(ITypeSymbol type)
    {
        for (var baseType = type.BaseType; baseType is not null && baseType.SpecialType != SpecialType.System_Object; baseType = baseType.BaseType)
        {
            yield return baseType;
        }
    }

    private static string? ReasonUnreadable(IParameterSymbol parameter)
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
