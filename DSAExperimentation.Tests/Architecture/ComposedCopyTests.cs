using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Tests.Architecture;

// ARCHITECTURE.md section 17.5 from the composed side: a composed arm composes the library, so it reads
// a value the library or the LeetCode tier's root states instead of declaring its own copy. A
// baseline's copy is what that section asks for, so every solution constant a baseline reaches is
// spared: what BaselineIsolationTests' sweep reaches, and what the other overloads of each method it
// reaches reach too, since a benchmark arm runs one overload of a strategy and LeetCode's shape is
// often another. LibraryValueCopies says what counts as a copy, and LibraryValueCopiesTests pins that
// down.
public sealed partial class ComposedCopyTests
{
    // Keyed by the copy's full name: a constant that repeats a shared value but means something else.
    private static readonly Dictionary<string, string> AllowedCopies = new(StringComparer.Ordinal)
    {
        ["DSAExperimentation.LeetCode.Solutions.ValidSquare.ValidSquareSolution.FourthSideIndex"] =
            "The fourth side's place among the sorted distances, not how many sides a polygon needs.",
    };

    private static readonly Lazy<IReadOnlyList<LibraryValueCopies.Copy>> Copies = new(FindCopies);

    [Fact]
    public void ComposedOnlyConstants_CopyNoSharedValue()
    {
        var offences = Copies.Value
            .Where(copy => !AllowedCopies.ContainsKey(copy.Name))
            .Select(copy => $"{copy.Name} repeats {copy.Source}; read it from there (ARCHITECTURE.md 17.5).")
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // An entry whose copy has gone would otherwise sit in the list excusing nothing.
    [Fact]
    public void AllowedCopies_AreAllStillCopies()
        => Assert.DoesNotContain(AllowedCopies.Keys, name => !Copies.Value.Any(copy => copy.Name == name));

    private static IReadOnlyList<LibraryValueCopies.Copy> FindCopies()
    {
        var traced = BaselineIsolationTests.Sweep.Value.SelectMany(armTrace => armTrace.Trace.SolutionMembers).ToList();
        var overloads = traced.OfType<IMethodSymbol>().SelectMany(method => method.ContainingType.GetMembers(method.Name));
        var models = new SourceModels([RepositoryCompilation.Benchmarks, RepositoryCompilation.Solutions, RepositoryCompilation.Framework]);
        var baselineReached = BaselineTrace.SolutionMembersReachedFrom(traced.Concat(overloads), models);

        return LibraryValueCopies.In(RepositoryCompilation.Framework, RepositoryCompilation.Solutions, baselineReached);
    }
}
