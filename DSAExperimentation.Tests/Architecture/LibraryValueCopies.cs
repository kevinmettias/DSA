using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Tests.Architecture;

// ARCHITECTURE.md section 17.5's other half: a composed arm composes the library, so a value the
// library already states is read from it rather than copied. A baseline must keep its own copy, so
// only a solution constant no baseline reaches is a candidate; the caller says which ones a baseline
// reaches. The shared values are the public constants of the library and of the LeetCode tier's root
// (LeetCodeModulus.Value). A copy is a constant declared in a problem's folder whose value equals one.
//
// Small numbers and single characters mean many things - a 2 is a halving factor in one file and a
// pair in the next - so for those the copy's name must also share a word with the shared constant's
// name or its type's. A larger number, a fraction or a string is distinctive enough on its value. The
// words compared skip the ones that name a role rather than a subject, such as Size or Count, and
// ignore a plural's s. An enum member is neither a copy nor a source: its number is how the enum is
// encoded, not a value anyone reads for itself.
internal static class LibraryValueCopies
{
    private const string SolutionNamespace = "DSAExperimentation.LeetCode";

    // Integers this far from zero are taken to mean one thing; nearer ones need a shared word too.
    private const decimal LargestCommonMagnitude = 10;

    private static readonly Regex WordPattern = new("[A-Z]+(?![a-z])|[A-Z]?[a-z]+|[0-9]+");

    private static readonly HashSet<string> RoleWords =
        new(["value", "size", "count", "factor", "max", "min", "limit", "default", "first", "last", "index", "length", "number"], StringComparer.Ordinal);

    // Each copy of a shared value, with the shared constant it copies, both by their full names.
    public static IReadOnlyList<Copy> In(Compilation framework, Compilation solutions, IReadOnlySet<ISymbol> baselineReached)
    {
        var shared = ConstantsIn(framework).Where(BaselineTrace.IsLibrary)
            .Concat(ConstantsIn(solutions).Where(IsTierRoot))
            .Where(IsVisibleEverywhere)
            .ToList();

        return [.. ConstantsIn(solutions)
            .Where(constant => IsProblemLocal(constant) && !baselineReached.Contains(constant))
            .SelectMany(copy => shared.Where(source => IsCopy(copy, source)).Select(source => new Copy(Describe(copy), Describe(source))))
            .OrderBy(copy => copy.Name, StringComparer.Ordinal)];
    }

    private static IEnumerable<IFieldSymbol> ConstantsIn(Compilation compilation)
        => TypesIn(compilation.Assembly.GlobalNamespace)
            .SelectMany(type => type.GetMembers().OfType<IFieldSymbol>())
            .Where(field => field is { IsConst: true, HasConstantValue: true, ContainingType.TypeKind: not TypeKind.Enum });

    private static IEnumerable<INamedTypeSymbol> TypesIn(INamespaceOrTypeSymbol container)
        => container.GetMembers().SelectMany(TypesDeclaredBy);

    private static IEnumerable<INamedTypeSymbol> TypesDeclaredBy(ISymbol member)
        => member switch
        {
            INamespaceSymbol space => TypesIn(space),
            INamedTypeSymbol type => TypesIn(type).Prepend(type),
            _ => Enumerable.Empty<INamedTypeSymbol>(),
        };

    private static bool IsTierRoot(IFieldSymbol field) => NamespaceOf(field) == SolutionNamespace;

    private static bool IsProblemLocal(IFieldSymbol field) => NamespaceOf(field).StartsWith($"{SolutionNamespace}.", StringComparison.Ordinal);

    private static string NamespaceOf(ISymbol symbol) => symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;

    // A composed arm can read only what is public all the way out.
    private static bool IsVisibleEverywhere(IFieldSymbol field)
    {
        for (ISymbol? at = field; at is not null and not INamespaceSymbol; at = at.ContainingSymbol)
        {
            if (at.DeclaredAccessibility != Accessibility.Public)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsCopy(IFieldSymbol copy, IFieldSymbol source)
    {
        var value = Normalized(copy.ConstantValue);

        if (value is null || !value.Equals(Normalized(source.ConstantValue)))
        {
            return false;
        }

        return IsDistinctive(value) || HasSubjectWordInCommon(copy, source);
    }

    // Integers compare across their widths, so an int copy of a long constant is still a copy.
    private static object? Normalized(object? value)
        => value switch
        {
            sbyte or byte or short or ushort or int or uint or long or ulong => Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture),
            float single => (double)single,
            double or string or char => value,
            _ => null,
        };

    private static bool IsDistinctive(object value)
        => value switch
        {
            decimal integer => Math.Abs(integer) > LargestCommonMagnitude,
            char => false,
            _ => true,
        };

    private static bool HasSubjectWordInCommon(IFieldSymbol copy, IFieldSymbol source)
    {
        var sourceWords = SubjectWords(source.Name).Concat(SubjectWords(source.ContainingType.Name)).ToHashSet(StringComparer.Ordinal);

        return SubjectWords(copy.Name).Any(sourceWords.Contains);
    }

    private static IEnumerable<string> SubjectWords(string name)
        => WordPattern.Matches(name)
            .Select(match => Singular(match.Value.ToLowerInvariant()))
            .Where(word => !RoleWords.Contains(word));

    private static string Singular(string word)
    {
        if (word.Length > 3 && word.EndsWith('s'))
        {
            return word[..^1];
        }

        return word;
    }

    private static string Describe(ISymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

    // One problem's constant, and the shared constant whose value it repeats.
    public sealed record Copy(string Name, string Source);
}
