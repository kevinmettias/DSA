using System.Collections.Immutable;
using DSAExperimentation.Domain.Language;

namespace DSAExperimentation.Tests.Domain.Language;

// Read through a test-only set of German's sch and ch, so the table is shown to be a language's and
// not English's alone. A span of a longer word is looked up in place, in either case.
public sealed partial class MultigraphSetTests
{
    public static TheoryData<string, int, int> SpansSpellingOne => new()
    {
        { "Schule", 0, 3 },
        { "Schule", 1, 2 },
        { "SCHULE", 0, 3 },
    };

    public static TheoryData<string, int, int> SpansSpellingNone => new()
    {
        { "Schule", 0, 2 },
        { "Schule", 2, 3 },
    };

    [Theory]
    [MemberData(nameof(SpansSpellingOne))]
    public void Has_SpanSpellingOneOfTheSet_IsTrue(string word, int start, int length) =>
        Assert.True(MultigraphSet<GermanMultigraphs>.Has(word.AsSpan(start, length)));

    [Theory]
    [MemberData(nameof(SpansSpellingNone))]
    public void Has_SpanSpellingNoneOfTheSet_IsFalse(string word, int start, int length) =>
        Assert.False(MultigraphSet<GermanMultigraphs>.Has(word.AsSpan(start, length)));

    private readonly struct GermanMultigraphs : IMultigraphSet
    {
        public static ImmutableArray<Multigraph> Multigraphs { get; } = [new("sch"), new("ch")];
    }
}
