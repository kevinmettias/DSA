using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// One strategy of an enrolled design solution as a generated arm calls it (ARCHITECTURE 17.13): a factory
// making the instance from the constructor's inputs, and for each of LeetCode's methods the method answering
// it - the question's Calls type's static method of the call's name, ignoring case, which takes the instance
// first, or else the instance's own - with the inputs it reads and the answer it gives. The harness's
// CatalogCalls binds the same methods at run time. A strategy that cannot be called this way is refused with
// the reason, which the generator reports.
internal sealed class ScriptShape(IMethodSymbol factory, IReadOnlyList<ScriptCall> calls)
{
    private const string Generic = "it is generic, so no one instantiation is the strategy";
    private const string NothingMade = "it makes no instance a script can call";

    // What separates the words of a few of LeetCode's method names, as in get_root.
    private const string WordBreak = "_";

    public string Name { get; } = factory.Name;

    // The constructor's inputs, as the factory takes them.
    public IReadOnlyList<string> Inputs { get; } = [.. factory.Parameters.Select(parameter => NotationTypes.Render(parameter.Type))];

    // The method answering each of LeetCode's methods, in the order the question's signature lists them.
    public IReadOnlyList<ScriptCall> Calls { get; } = calls;

    // The arm a factory makes, or why it makes none.
    public static (ScriptShape? Shape, string? Reason) Of(IMethodSymbol factory, INamedTypeSymbol? stated, DesignShape design)
    {
        var made = factory.ReturnType;
        var reason = ReasonUnbound(factory, design) ?? design.Methods.Select(method => ReasonCallUnbound(made, stated, method)).FirstOrDefault(refusal => refusal is not null);

        if (reason is not null)
        {
            return (null, reason);
        }

        var bound = design.Methods.Select(method => CallOf(made, stated, method)).OfType<ScriptCall>().ToList();

        return (new ScriptShape(factory, bound), null);
    }

    private static string? ReasonUnbound(IMethodSymbol factory, DesignShape design)
    {
        if (factory.IsGenericMethod)
        {
            return Generic;
        }

        var makesNothingToCall = factory.ReturnsVoid || factory.ReturnsByRef || factory.ReturnType.IsRefLikeType;

        if (makesNothingToCall)
        {
            return NothingMade;
        }

        if (factory.Parameters.Length != design.ConstructorInputs)
        {
            return $"it takes {factory.Parameters.Length} inputs, and LeetCode's constructor takes {design.ConstructorInputs}";
        }

        return factory.Parameters.Select(ScriptCall.ReasonUnreadable).FirstOrDefault(unreadable => unreadable is not null);
    }

    private static string? ReasonCallUnbound(ITypeSymbol made, INamedTypeSymbol? stated, (string Name, int Inputs) method)
    {
        if (CallOf(made, stated, method) is not { } call)
        {
            return $"its {made.Name} answers no {method.Name} call of {method.Inputs} inputs by one method, and the question's Calls type states none";
        }

        return call.ReasonUnbound;
    }

    // The call answering one of LeetCode's methods on the instance: the Calls type's method for it, or else
    // the instance's own; null when neither has exactly one.
    private static ScriptCall? CallOf(ITypeSymbol made, INamedTypeSymbol? stated, (string Name, int Inputs) method)
    {
        if (StatedAnswererOf(made, stated, method) is { } statedAnswerer)
        {
            var statedOn = NotationTypes.Render(statedAnswerer.ContainingType);
            return new ScriptCall(method.Name, statedAnswerer, statedOn);
        }

        if (OwnAnswererOf(made, method) is { } own)
        {
            return new ScriptCall(method.Name, own, statedOn: null);
        }

        return null;
    }

    private static IMethodSymbol? StatedAnswererOf(ITypeSymbol made, INamedTypeSymbol? stated, (string Name, int Inputs) method)
    {
        if (stated is null)
        {
            return null;
        }

        var candidates = stated.GetMembers().OfType<IMethodSymbol>()
            .Where(candidate => candidate is { IsStatic: true, MethodKind: MethodKind.Ordinary } && IsNamedFor(candidate, method.Name) && IsStatedFor(candidate, made, method.Inputs))
            .ToList();

        return TheOneOf(candidates);
    }

    // Named for the call ignoring case, and the underscores LeetCode writes in a few names (get_root).
    private static bool IsNamedFor(IMethodSymbol candidate, string name)
    {
        var joined = name.Replace(WordBreak, string.Empty);

        return string.Equals(candidate.Name, joined, StringComparison.OrdinalIgnoreCase);
    }

    // Whether a Calls method takes the made instance, then the call's arguments.
    private static bool IsStatedFor(IMethodSymbol candidate, ITypeSymbol made, int inputs) =>
        candidate.Parameters.Length == inputs + 1 && IsAssignableTo(made, candidate.Parameters[0].Type);

    // The made type is the parameter's, or one of the classes or interfaces it extends.
    private static bool IsAssignableTo(ITypeSymbol made, ITypeSymbol target) =>
        SymbolEqualityComparer.Default.Equals(made, target)
            || made.AllInterfaces.Any(contract => SymbolEqualityComparer.Default.Equals(contract, target))
            || BaseTypesOf(made).Any(baseType => SymbolEqualityComparer.Default.Equals(baseType, target));

    private static IEnumerable<ITypeSymbol> BaseTypesOf(ITypeSymbol type)
    {
        for (var baseType = type.BaseType; baseType is not null && baseType.SpecialType != SpecialType.System_Object; baseType = baseType.BaseType)
        {
            yield return baseType;
        }
    }

    private static IMethodSymbol? TheOneOf(List<IMethodSymbol> candidates)
    {
        if (candidates.Count != 1)
        {
            return null;
        }

        return candidates[0];
    }

    private static IMethodSymbol? OwnAnswererOf(ITypeSymbol made, (string Name, int Inputs) method)
    {
        var candidates = InstanceMethodsOf(made)
            .Where(candidate => IsNamedFor(candidate, method.Name) && candidate.Parameters.Length == method.Inputs)
            .ToList();

        return TheOneOf(candidates);
    }

    // The public or internal instance methods of the made type: an interface's with those of the interfaces
    // it extends, a class's with those it inherits, each override counted once, as the method it overrides.
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
}
