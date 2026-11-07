using DSAExperimentation.DataStructures.SubstringIndex;
using DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;
using SuffixArrayIndex = DSAExperimentation.DataStructures.SuffixArray.SuffixArrayIndex;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SubstringIndex;

// ILocatingSubstringIndex's contract over the suffix array (with its text) and the suffix tree, the
// two structures that answer it; the automaton does not. Positions come in no promised order, so
// they are compared sorted.
public sealed partial class LocatingSubstringIndexTests
{
    private static readonly Dictionary<string, Func<string, ILocatingSubstringIndex>> Indexes =
        new()
        {
            ["suffix array"] = text => new SuffixArrayIndex(text),
            ["suffix tree"] = text => new SuffixTreeStructure(text),
        };

    public static TheoryData<string, string> Cases =>
        SubstringIndexTexts.EveryTextFor(Indexes.Keys);

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindOccurrences_EveryPattern_ListsWhatAScanFinds(
        string index, string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var built = Indexes[index](text);

        var wrong = SubstringPatterns.PatternsFor(text)
            .Where(pattern => !IsWhatAScanFinds(text, pattern, built.FindOccurrences(pattern)))
            .ToList();

        Assert.Empty(wrong);
    }

    private static bool IsWhatAScanFinds(string text, string pattern, IEnumerable<int> starts) =>
        starts.Order().SequenceEqual(BruteForceSubstrings.Starts(text, pattern));
}
