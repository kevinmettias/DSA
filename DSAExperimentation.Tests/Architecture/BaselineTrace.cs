using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DSAExperimentation.Tests.Architecture;

// ARCHITECTURE.md section 17.5's baseline trace, read off the compiled code rather than its
// spelling: everything a benchmark's Baseline = true arm runs, and every library name the solution
// code among it binds to.
//
// The arm and the benchmark helpers it calls are the harness, handing the baseline its input, so
// the trace walks them only to find where the baseline starts:
// - each solution member they reach is an entry, and the types an entry's parameters name are its
//   input, which the baseline may use and whose bodies are not traced;
// - a benchmark field they read is followed through every value the benchmark class assigns it,
//   into the solution code and the types that value creates, so an arm that calls a method on an
//   object [GlobalSetup] built starts where that object was made;
// - Fixtures/ only builds input, and is not walked.
// Solution code reached from an entry is the baseline, and each library name it binds to is
// charged to the arm:
// - calls, constructions, method groups, and field and property reads, each followed into its
//   declaration;
// - a static field's initializer once the field is read, a static constructor once anything
//   touches its type, and an instance field's initializer whenever its type is created;
// - every type an operation produces or declares, so a var-inferred local is read like a named one;
// - a virtual or interface call, resolved against the types the trace has seen created (rapid type
//   analysis), and each created solution type's overrides of members declared outside the source,
//   which the runtime calls back into.
// LeetCode's own I/O shapes (ContractTypes) are input wherever they appear.
internal sealed class BaselineTrace(SourceModels models, HashSet<ISymbol> knownInput)
{
    private const string SolutionNamespace = "DSAExperimentation.LeetCode.Solutions";
    private const string BenchmarkNamespace = "DSAExperimentation.Benchmarks";
    private const string FixtureNamespace = "DSAExperimentation.Benchmarks.Fixtures";

    // The library a baseline may not name: every tier of the framework project.
    private static readonly string[] LibraryNamespaces =
    [
        "DSAExperimentation.DataStructures",
        "DSAExperimentation.Algorithms",
        "DSAExperimentation.Mechanisms",
        "DSAExperimentation.Domain",
    ];

    // The library types that stand for LeetCode's own I/O shapes. A baseline that takes, returns or
    // builds one answers in LeetCode's terms rather than composing the library.
    private static readonly HashSet<string> ContractTypes =
        new(["BinaryTreeNode", "SinglyLinkedListNode", "QuadTreeNode", "ICache"], StringComparer.Ordinal);

    private readonly HashSet<ISymbol> _knownInput = knownInput;
    private readonly HashSet<ISymbol> _inputTypes = new(SymbolEqualityComparer.Default);
    private readonly Queue<ISymbol> _pending = new();
    private readonly Dictionary<ISymbol, ISymbol?> _reachedFrom = new(SymbolEqualityComparer.Default);
    private readonly HashSet<ISymbol> _typesInitialized = new(SymbolEqualityComparer.Default);
    private readonly HashSet<ISymbol> _instantiated = new(SymbolEqualityComparer.Default);
    private readonly HashSet<IMethodSymbol> _virtualTargets = new(SymbolEqualityComparer.Default);
    private readonly HashSet<ISymbol> _slicedMembers = new(SymbolEqualityComparer.Default);
    private readonly List<(ISymbol Library, ISymbol Site)> _charged = [];
    private readonly HashSet<ISymbol> _entries = new(SymbolEqualityComparer.Default);

    // Every Baseline = true arm declared under a ProblemSolutions folder of the benchmark compilation.
    public static IEnumerable<IMethodSymbol> ArmsIn(Compilation benchmarks)
        => benchmarks.SyntaxTrees
            .Where(tree => tree.FilePath.Contains("ProblemSolutions", StringComparison.Ordinal))
            .SelectMany(tree =>
            {
                var model = benchmarks.GetSemanticModel(tree);

                return tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Select(method => model.GetDeclaredSymbol(method))
                    .OfType<IMethodSymbol>()
                    .Where(IsBaselineArm);
            });

    // The solution members one arm's harness reaches, and the library names its baseline reaches,
    // each with the chain of members that reached it, arm first. An entry found late can name input
    // an earlier step already traced into, so the trace runs again with every input it found, until
    // a run finds no input the one before it did not know.
    public static ArmTrace From(IMethodSymbol arm, SourceModels models)
    {
        var inputSoFar = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
        BaselineTrace trace;
        bool foundNewInput;

        do
        {
            trace = new BaselineTrace(models, inputSoFar);
            trace.Reach(arm, from: null);
            trace.Drain();
            foundNewInput = !trace._inputTypes.IsSubsetOf(inputSoFar);
            inputSoFar.UnionWith(trace._inputTypes);
        }
        while (foundNewInput);

        return new ArmTrace(
            [.. trace._entries.Select(Describe).Order(StringComparer.Ordinal)],
            trace.Reaches(),
            trace._reachedFrom.Keys.Where(IsSolution).ToHashSet(SymbolEqualityComparer.Default));
    }

    // Every solution member reachable from the given ones, each read as baseline code: what a
    // baseline's own copies are, when a caller wants more of the baseline than one arm runs.
    public static IReadOnlySet<ISymbol> SolutionMembersReachedFrom(IEnumerable<ISymbol> starts, SourceModels models)
    {
        var trace = new BaselineTrace(models, new HashSet<ISymbol>(SymbolEqualityComparer.Default));

        foreach (var start in starts)
        {
            trace.Reach(start.OriginalDefinition, from: null);
        }

        trace.Drain();

        return trace._reachedFrom.Keys.Where(IsSolution).ToHashSet(SymbolEqualityComparer.Default);
    }

    private static bool IsBaselineArm(IMethodSymbol method)
        => method.GetAttributes().Any(attribute =>
            attribute.AttributeClass?.Name == "BenchmarkAttribute"
            && attribute.NamedArguments.Any(argument => argument is { Key: "Baseline", Value.Value: true }));

    // Called by ReachWalker for each member the code it walks binds to. A library member is
    // charged if the baseline binds to it; a member of the input is neither charged nor traced; a
    // solution member is traced, and is an entry when the harness reaches it; a benchmark member is
    // traced only while the walk reads the harness itself.
    public void Bound(ISymbol member, ISymbol site, ReachMode mode)
    {
        TypesOfMemberBound(member, site, mode);

        if (member is IMethodSymbol method)
        {
            Dispatched(method, site);
        }

        Follow(member, site, mode);
    }

    private void Follow(ISymbol member, ISymbol site, ReachMode mode)
    {
        var definition = member.OriginalDefinition;

        if (IsInput(definition.ContainingType))
        {
            InputMemberRead(member);
        }
        else if (IsLibrary(definition))
        {
            Charge(definition, site, mode);
        }
        else if (IsSolution(definition))
        {
            SolutionMemberReached(member, site, mode);
        }
        else if (IsHarnessCode(definition) && mode == ReachMode.Harness)
        {
            Reach(definition, site);
        }
    }

    // The types a member reference names besides its own: the type that declares it, and a generic
    // method's type arguments, each of which is a witness the code chose.
    private void TypesOfMemberBound(ISymbol member, ISymbol site, ReachMode mode)
    {
        if (member.ContainingType is { } owner)
        {
            TypeBound(owner, site, mode);
        }

        if (member is IMethodSymbol generic)
        {
            foreach (var argument in generic.TypeArguments)
            {
                TypeBound(argument, site, mode);
                Instantiated(argument, site);
            }
        }
    }

    // A call the runtime dispatches, met for the first time: resolved against every type created so
    // far, and remembered for every type created later.
    private void Dispatched(IMethodSymbol method, ISymbol site)
    {
        if (!IsDispatched(method) || !_virtualTargets.Add(method))
        {
            return;
        }

        foreach (var created in _instantiated.OfType<INamedTypeSymbol>().ToList())
        {
            ImplementationIn(created, method, site);
        }
    }

    private void SolutionMemberReached(ISymbol member, ISymbol site, ReachMode mode)
    {
        if (mode != ReachMode.Baseline && member is IMethodSymbol entry)
        {
            EntryReached(entry);
        }

        Reach(member.OriginalDefinition, site);
    }

    // Called by ReachWalker for a type the code it walks produces, declares or names. A type
    // argument of a solution type is a witness the code chose, and counts as created.
    public void TypeBound(ITypeSymbol type, ISymbol site, ReachMode mode)
    {
        foreach (var library in TypesIn(type).Where(IsLibrary))
        {
            Charge(library, site, mode);
        }

        if (type is INamedTypeSymbol { TypeArguments.Length: > 0 } constructed && IsSolution(constructed.OriginalDefinition))
        {
            foreach (var argument in constructed.TypeArguments)
            {
                Instantiated(argument, site);
            }
        }

        if (mode == ReachMode.Baseline)
        {
            foreach (var solutionType in TypesIn(type).Where(IsSolution))
            {
                RunStaticConstructors(solutionType, site);
            }
        }
    }

    // Called by ReachWalker when the code it walks creates an object, and by the trace for a type
    // argument that chooses a witness.
    public void Instantiated(ITypeSymbol type, ISymbol site)
    {
        if (type is not INamedTypeSymbol named || !_instantiated.Add(named.OriginalDefinition))
        {
            return;
        }

        foreach (var target in _virtualTargets.ToList())
        {
            ImplementationIn(named, target, site);
        }

        if (IsSolution(named.OriginalDefinition) && !IsInput(named))
        {
            SolutionTypeCreated(named, site);
        }
    }

    // A solution type the baseline creates runs its instance initializers with every constructor,
    // and is called back by the runtime through the members it inherits from outside the source.
    private void SolutionTypeCreated(INamedTypeSymbol type, ISymbol site)
    {
        foreach (var callback in CallbacksOf(type))
        {
            Reach(callback.OriginalDefinition, site);
        }

        foreach (var initializer in models.InstanceInitializersOf(type.OriginalDefinition))
        {
            Walk(initializer, type.OriginalDefinition, ReachMode.Baseline);
        }
    }

    // Called by ReachWalker for a benchmark field or property the harness reads: every value its
    // class assigns it is followed as a slice.
    public void BenchmarkMemberRead(ISymbol member, ISymbol site)
    {
        if (_slicedMembers.Add(member.OriginalDefinition))
        {
            foreach (var value in models.ValuesAssignedTo(member.OriginalDefinition))
            {
                Walk(value, site, ReachMode.Slice);
            }
        }
    }

    private void EntryReached(IMethodSymbol entry)
    {
        _entries.Add(entry.OriginalDefinition);

        foreach (var parameter in entry.Parameters)
        {
            _inputTypes.UnionWith(TypesIn(parameter.Type));
        }
    }

    // What a member of the input hands back is input too: the container the harness chose holds it.
    private void InputMemberRead(ISymbol member)
    {
        ITypeSymbol? type = member switch
        {
            IMethodSymbol method => method.ReturnType,
            IPropertySymbol property => property.Type,
            IFieldSymbol field => field.Type,
            _ => null,
        };

        if (type is not null)
        {
            _inputTypes.UnionWith(TypesIn(type));
        }
    }

    private bool IsInput(INamedTypeSymbol? type)
        => type is not null && (_knownInput.Contains(type.OriginalDefinition) || (ContractTypes.Contains(type.Name) && IsLibrary(type)));

    private void Reach(ISymbol member, ISymbol? from)
    {
        if (_reachedFrom.TryAdd(member, from))
        {
            _pending.Enqueue(member);
        }
    }

    // Each pending member's bodies, read as the harness's or the baseline's by where it is declared.
    private void Drain()
    {
        while (_pending.TryDequeue(out var member))
        {
            var mode = IsBenchmark(member) ? ReachMode.Harness : ReachMode.Baseline;

            foreach (var body in models.BodiesOf(member))
            {
                Walk(body, member, mode);
            }
        }
    }

    private void Walk(IOperation operation, ISymbol site, ReachMode mode) => new ReachWalker(this, site, mode).Visit(operation);

    // A static constructor runs once code touches its type. A static field's initializer is traced
    // only once that field is read, since what else the type holds may serve another arm.
    private void RunStaticConstructors(INamedTypeSymbol type, ISymbol site)
    {
        if (_typesInitialized.Add(type))
        {
            foreach (var constructor in type.StaticConstructors)
            {
                Reach(constructor, site);
            }
        }
    }

    private void Charge(ISymbol library, ISymbol site, ReachMode mode)
    {
        if (mode == ReachMode.Baseline)
        {
            _charged.Add((library, site));
        }
    }

    private static bool IsDispatched(IMethodSymbol method)
        => method.IsVirtual || method.IsAbstract || method.IsOverride || method.ContainingType?.TypeKind == TypeKind.Interface;

    // Where a dispatched call lands in one created type. A library type the harness created is
    // input, so a call declared outside the source and the library that lands in one runs on input.
    private void ImplementationIn(INamedTypeSymbol type, IMethodSymbol target, ISymbol site)
    {
        if (LandingIn(type, target)?.OriginalDefinition is not { } definition)
        {
            return;
        }

        if (IsLibrary(definition) && IsDeclaredInRepository(target.OriginalDefinition))
        {
            Charge(definition, site, ReachMode.Baseline);
        }
        else if (IsSolution(definition) && !IsInput(definition.ContainingType))
        {
            Reach(definition, site);
        }
    }

    private static IMethodSymbol? LandingIn(INamedTypeSymbol type, IMethodSymbol target)
    {
        if (target.ContainingType?.TypeKind == TypeKind.Interface)
        {
            return type.FindImplementationForInterfaceMember(target) as IMethodSymbol;
        }

        return OverrideIn(type, target);
    }

    // The member of a type, or of the nearest base that declares one, overriding a virtual method.
    private static IMethodSymbol? OverrideIn(INamedTypeSymbol type, IMethodSymbol target)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var found = current.GetMembers(target.Name).OfType<IMethodSymbol>().FirstOrDefault(candidate => IsOverrideOf(candidate, target));

            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    // Whether a method is a virtual method's own declaration or overrides it, however many bases up.
    private static bool IsOverrideOf(IMethodSymbol candidate, IMethodSymbol target)
    {
        for (var overridden = candidate; overridden is not null; overridden = overridden.OverriddenMethod)
        {
            if (SymbolEqualityComparer.Default.Equals(overridden.OriginalDefinition, target.OriginalDefinition))
            {
                return true;
            }
        }

        return false;
    }

    // The members of a created type that outside code calls back into: its implementations of
    // interfaces declared outside the source, and its overrides of methods declared there.
    private static IEnumerable<IMethodSymbol> CallbacksOf(INamedTypeSymbol type)
    {
        var implementations = type.AllInterfaces
            .Where(contract => !IsSource(contract.OriginalDefinition))
            .SelectMany(contract => contract.GetMembers().OfType<IMethodSymbol>())
            .Select(member => type.FindImplementationForInterfaceMember(member))
            .OfType<IMethodSymbol>();
        var overrides = type.GetMembers().OfType<IMethodSymbol>()
            .Where(method => method.IsOverride && method.OverriddenMethod is { } overridden && !IsSource(overridden.OriginalDefinition));

        return implementations.Concat(overrides).Where(method => IsSource(method.OriginalDefinition));
    }

    private List<LibraryReach> Reaches()
        => [.. _charged
            .Where(charge => !IsInput(charge.Library as INamedTypeSymbol ?? charge.Library.ContainingType))
            .GroupBy(charge => Describe(charge.Library), StringComparer.Ordinal)
            .Select(group => new LibraryReach(group.Key, ChainTo(group.First().Site)))
            .OrderBy(reach => reach.Name, StringComparer.Ordinal)];

    private List<string> ChainTo(ISymbol site)
    {
        var chain = new List<string>();

        for (ISymbol? at = site; at is not null; at = _reachedFrom.GetValueOrDefault(at))
        {
            chain.Add(Describe(at));
        }

        chain.Reverse();

        return chain;
    }

    private static string Describe(ISymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

    // The named types a type is built from, as their definitions: itself, its type arguments and
    // those of the types it is nested in, an array's elements. The enclosing types themselves are
    // not among them: a record nested in a solution class does not make the class its input.
    private static IEnumerable<INamedTypeSymbol> TypesIn(ITypeSymbol type)
    {
        switch (type)
        {
            case IArrayTypeSymbol array:
                return TypesIn(array.ElementType);
            case IPointerTypeSymbol pointer:
                return TypesIn(pointer.PointedAtType);
            case INamedTypeSymbol named:
                var arguments = named.TypeArguments.AsEnumerable();

                for (var outer = named.ContainingType; outer is not null; outer = outer.ContainingType)
                {
                    arguments = arguments.Concat(outer.TypeArguments);
                }

                return arguments.SelectMany(TypesIn).Prepend(named.OriginalDefinition);
            default:
                return [];
        }
    }

    public static bool IsBenchmark(ISymbol symbol) => IsIn(symbol, BenchmarkNamespace);

    private static bool IsSolution(ISymbol symbol) => IsIn(symbol, SolutionNamespace);

    private static bool IsSource(ISymbol symbol) => IsSolution(symbol) || IsBenchmark(symbol);

    private static bool IsDeclaredInRepository(ISymbol symbol) => IsSource(symbol) || IsLibrary(symbol);

    // Benchmark code that runs with the arm, as opposed to Fixtures/, which only lays input out.
    private static bool IsHarnessCode(ISymbol symbol) => IsBenchmark(symbol) && !IsIn(symbol, FixtureNamespace);

    public static bool IsLibrary(ISymbol symbol) => Array.Exists(LibraryNamespaces, library => IsIn(symbol, library));

    private static bool IsIn(ISymbol symbol, string container)
    {
        var name = (symbol as INamespaceSymbol ?? symbol.ContainingNamespace)?.ToDisplayString() ?? string.Empty;

        return name == container || name.StartsWith($"{container}.", StringComparison.Ordinal);
    }

    // What one arm's trace found: the solution members its harness reaches, the library names its
    // baseline reaches, and every solution member the baseline runs or reads, its constants among them.
    public sealed record ArmTrace(IReadOnlyList<string> Entries, IReadOnlyList<LibraryReach> Reaches, IReadOnlySet<ISymbol> SolutionMembers);

    // One library name a baseline reaches, and the members that led there, arm first.
    public sealed record LibraryReach(string Name, IReadOnlyList<string> Chain);
}
