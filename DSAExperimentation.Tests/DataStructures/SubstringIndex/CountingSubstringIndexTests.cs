using DSAExperimentation.DataStructures.SubstringIndex;
using DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;
using SuffixArrayIndex = DSAExperimentation.DataStructures.SuffixArray.SuffixArrayIndex;
using SuffixAutomatonStructure = DSAExperimentation.DataStructures.SuffixAutomaton.SuffixAutomaton;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SubstringIndex;

// ICountingSubstringIndex's contract over the suffix array (with its text), the suffix tree and the
// suffix automaton: every count a scan of the text gives, the empty pattern's n + 1 among them.
public sealed partial class CountingSubstringIndexTests
{
    private static readonly Dictionary<string, Func<string, ICountingSubstringIndex>> Indexes =
        new()
        {
            ["suffix array"] = text => new SuffixArrayIndex(text),
            ["suffix tree"] = text => new SuffixTreeStructure(text),
            ["suffix automaton"] = text => new SuffixAutomatonStructure(text),
        };

    public static TheoryData<string, string> Cases =>
        SubstringIndexTexts.EveryTextFor(Indexes.Keys);

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountOccurrences_EveryPattern_CountsWhatAScanFinds(
        string index, string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var built = Indexes[index](text);

        var wrong = SubstringPatterns.PatternsFor(text)
            .Where(pattern => built.CountOccurrences(pattern) != CountByScan(text, pattern))
            .ToList();

        Assert.Empty(wrong);
    }

    private static int CountByScan(string text, string pattern) =>
        BruteForceSubstrings.Starts(text, pattern).Count;
}
