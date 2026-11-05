using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// Writes one benchmark class per solution enrolled in the catalog harness (ARCHITECTURE 17.13) when the
// benchmarks build, so no problem needs a hand-written class to be timed over the cases the LeetCode
// catalog holds for it. A solution enrolls by carrying LeetCodeQuestionAttribute; each of its strategies,
// the <Operation>By<Strategy> methods of 17.3, becomes an arm that runs over every case per call, and the
// first declared is the baseline. The class is ordinary C# - its arms call the strategy directly, with no
// delegate or reflection between the loop and the call - so the category discoverer, the generic arm
// checks and --filter treat it like any other benchmark class.
//
// A strategy no arm can call - generic, answering in place, or taking a type LeetCode's notation cannot be
// read into - is reported as an error naming why, rather than left out of its class: an enrolled solution
// whose arm went missing would look measured and not be.

/// <summary>
/// Writes a <c>&lt;Problem&gt;CatalogBenchmarks</c> class for every solution class that carries
/// <c>LeetCodeQuestionAttribute</c>, with one arm per strategy timed over the LeetCode catalog's cases.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class CatalogBenchmarkGenerator : IIncrementalGenerator
{
    private const string QuestionAttribute = "DSAExperimentation.LeetCode.LeetCodeQuestionAttribute";
    private const string SolutionSuffix = "Solution";
    private const string ClassSuffix = "CatalogBenchmarks";
    private const string StrategyInfix = "By";
    private const string NoOverloadBinds = "no overload takes a case's inputs";
    private const string RewritesInputArgument = "RewritesInput";
    private const string AnswersInPlaceArgument = "AnswersInPlace";

    // Where the catalog keeps one file per question, whose signature names the question's declared inputs.
    private const string QuestionFolder = "/Snapshot/Questions/";
    private const string QuestionExtension = ".json";
    private const char WindowsSeparator = '\\';
    private const char Separator = '/';

    private static readonly DiagnosticDescriptor UnboundStrategy = new(
        id: "DSACAT001",
        title: "A strategy of an enrolled solution cannot be a catalog benchmark arm",
        messageFormat: "{0}.{1} cannot be a catalog benchmark arm: {2}",
        category: "CatalogHarness",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>Registers the generation, which reruns whenever the compilation changes.</summary>
    /// <param name="context">The compiler's context for this generator.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var signatures = context.AdditionalTextsProvider
            .Where(static text => IsQuestionFile(text.Path))
            .Select(static (text, cancellation) => (Slug: Path.GetFileNameWithoutExtension(text.Path), Declared: LeetCodeShape.DeclaredTypesIn(text.GetText(cancellation)?.ToString() ?? string.Empty)))
            .Collect();
        var classes = context.CompilationProvider.Combine(signatures).Select(static (pair, _) => ClassesFor(pair.Left, pair.Right));

        context.RegisterSourceOutput(classes, static (output, generated) => Emit(output, generated));
    }

    private static bool IsQuestionFile(string path)
    {
        var normalized = path.Replace(WindowsSeparator, Separator);
        return normalized.Contains(QuestionFolder) && normalized.EndsWith(QuestionExtension, StringComparison.Ordinal);
    }

    private static GeneratedClasses ClassesFor(Compilation compilation, ImmutableArray<(string Slug, IReadOnlyList<string> Declared)> signatures)
    {
        var generated = new GeneratedClasses();

        if (compilation.GetTypeByMetadataName(QuestionAttribute) is not { } attribute)
        {
            return generated;
        }

        var solutions = AssembliesThatCanEnroll(compilation, attribute.ContainingAssembly)
            .SelectMany(assembly => TypesIn(assembly.GlobalNamespace))
            .Where(type => IsEnrolled(type, attribute));

        var declaredBySlug = signatures.ToDictionary(signature => signature.Slug, signature => signature.Declared, StringComparer.Ordinal);

        foreach (var solution in solutions)
        {
            var declared = DeclaredTypesFor(solution, attribute, declaredBySlug);
            AddClassFor(solution, attribute, declared, generated);
        }

        return generated;
    }

    // The assembly declaring the attribute, and every referenced assembly that compiles against it, as the
    // solutions project does. LeetCode's conventions declare it, below the solutions that carry it.
    private static IEnumerable<IAssemblySymbol> AssembliesThatCanEnroll(Compilation compilation, IAssemblySymbol declaring) =>
        compilation.SourceModule.ReferencedAssemblySymbols
            .Where(assembly => IsCompiledAgainst(assembly, declaring))
            .Prepend(declaring);

    private static bool IsCompiledAgainst(IAssemblySymbol assembly, IAssemblySymbol declaring) =>
        assembly.Modules.Any(module => module.ReferencedAssemblies.Any(identity => identity.Name == declaring.Identity.Name));

    private static IEnumerable<INamedTypeSymbol> TypesIn(INamespaceSymbol space) =>
        space.GetTypeMembers().Concat(space.GetNamespaceMembers().SelectMany(TypesIn));

    private static bool IsEnrolled(INamedTypeSymbol type, INamedTypeSymbol attribute) =>
        type.GetAttributes().Any(applied => SymbolEqualityComparer.Default.Equals(applied.AttributeClass, attribute));

    // The inputs the solution's question declares, by the slug its attribute names; null when the benchmarks
    // build was handed no file for it.
    private static IReadOnlyList<string>? DeclaredTypesFor(INamedTypeSymbol solution, INamedTypeSymbol attribute, Dictionary<string, IReadOnlyList<string>> declaredBySlug)
    {
        var applied = solution.GetAttributes().First(candidate => SymbolEqualityComparer.Default.Equals(candidate.AttributeClass, attribute));
        var slug = applied.ConstructorArguments.FirstOrDefault().Value as string;
        return slug is not null && declaredBySlug.TryGetValue(slug, out var declared) ? declared : null;
    }

    private static void AddClassFor(INamedTypeSymbol solution, INamedTypeSymbol attribute, IReadOnlyList<string>? declared, GeneratedClasses generated)
    {
        var source = IsSet(solution, attribute, AnswersInPlaceArgument) ? AnswerSource.FirstInput : AnswerSource.ReturnValue;
        var arms = ArmsOf(solution, source, declared, generated);

        // A strategy that answers in place rewrites its input, so each call needs a copy of its own too.
        var name = NameFor(solution);
        var copiesInputs = source == AnswerSource.FirstInput || IsSet(solution, attribute, RewritesInputArgument);
        var writer = new CatalogBenchmarkWriter(NotationTypes.Render(solution), name, arms, copiesInputs);
        generated.Sources.Add((name, writer.Write()));
    }

    // Whether the solution's attribute sets a flag, such as RewritesInput: its strategies may rewrite the
    // inputs they are handed.
    private static bool IsSet(INamedTypeSymbol solution, INamedTypeSymbol attribute, string flag) =>
        solution.GetAttributes()
            .Where(applied => SymbolEqualityComparer.Default.Equals(applied.AttributeClass, attribute))
            .SelectMany(applied => applied.NamedArguments)
            .Any(argument => argument.Key == flag && argument.Value.Value is true);

    // One arm per strategy whose overloads settle on one a case can feed; a refusal for each other strategy.
    private static List<StrategyShape> ArmsOf(INamedTypeSymbol solution, AnswerSource source, IReadOnlyList<string>? declared, GeneratedClasses generated)
    {
        var arms = new List<StrategyShape>();

        foreach (var overloads in solution.GetMembers().OfType<IMethodSymbol>().Where(IsStrategy).GroupBy(method => method.Name))
        {
            var bound = BoundOverloadsOf([.. overloads], source, declared);

            if (bound.Count == 1)
            {
                var arm = StrategyShape.Of(bound[0], source);
                arms.Add(arm);
            }
            else
            {
                generated.Refusals.Add((solution.Name, overloads.Key, ReasonNoneOrSeveral(overloads.ToList(), bound.Count, source)));
            }
        }

        return arms;
    }

    // The overloads a case can feed; of several, those LeetCode's signature shapes (17.4).
    private static IReadOnlyList<IMethodSymbol> BoundOverloadsOf(List<IMethodSymbol> overloads, AnswerSource source, IReadOnlyList<string>? declared)
    {
        IReadOnlyList<IMethodSymbol> bound = [.. overloads.Where(method => StrategyShape.ReasonUnbound(method, source) is null)];

        if (bound.Count > 1 && declared is not null)
        {
            return LeetCodeShape.Narrowed(bound, declared);
        }

        return bound;
    }

    private static string ReasonNoneOrSeveral(List<IMethodSymbol> overloads, int boundCount, AnswerSource source)
    {
        if (boundCount > 0)
        {
            return $"{boundCount} of its overloads take a case's inputs, so which one is the arm is not settled";
        }

        return StrategyShape.ReasonUnbound(overloads[0], source) ?? NoOverloadBinds;
    }

    private static string NameFor(INamedTypeSymbol solution)
    {
        if (!solution.Name.EndsWith(SolutionSuffix, StringComparison.Ordinal))
        {
            return solution.Name + ClassSuffix;
        }

        return solution.Name.Substring(0, solution.Name.Length - SolutionSuffix.Length) + ClassSuffix;
    }

    private static void Emit(SourceProductionContext output, GeneratedClasses generated)
    {
        foreach (var (name, source) in generated.Sources)
        {
            output.AddSource($"{name}.g.cs", source);
        }

        foreach (var (solution, strategy, reason) in generated.Refusals)
        {
            var refusal = Diagnostic.Create(UnboundStrategy, Location.None, solution, strategy, reason);
            output.ReportDiagnostic(refusal);
        }
    }

    // A non-private static method named <Operation>By<Strategy>, as 17.3 names every strategy. A static local
    // function, read from the solutions assembly's metadata, is an internal method named for the strategy
    // around it (<CountByLoop>g__Step|0_0), which no code can name: it is not a strategy of its own.
    private static bool IsStrategy(IMethodSymbol method) =>
        method is { IsStatic: true, MethodKind: MethodKind.Ordinary, DeclaredAccessibility: Accessibility.Public or Accessibility.Internal, CanBeReferencedByName: true }
            && IsStrategyNamed(method.Name);

    // A `By` after the first character, starting a new word.
    private static bool IsStrategyNamed(string name)
    {
        for (var index = name.IndexOf(StrategyInfix, StringComparison.Ordinal);
            index > 0;
            index = name.IndexOf(StrategyInfix, index + 1, StringComparison.Ordinal))
        {
            var start = index + StrategyInfix.Length;

            if (start < name.Length && char.IsUpper(name[start]))
            {
                return true;
            }
        }

        return false;
    }

    // What one compilation yields: a source per enrolled solution, and every strategy no arm can call.
    private sealed record GeneratedClasses
    {
        public List<(string Name, string Source)> Sources { get; } = [];

        public List<(string Solution, string Strategy, string Reason)> Refusals { get; } = [];
    }
}
