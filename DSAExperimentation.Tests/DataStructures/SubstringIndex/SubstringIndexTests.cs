using DSAExperimentation.DataStructures.SubstringIndex;
using DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;
using SuffixArrayIndex = DSAExperimentation.DataStructures.SuffixArray.SuffixArrayIndex;
using SuffixAutomatonStructure = DSAExperimentation.DataStructures.SuffixAutomaton.SuffixAutomaton;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SubstringIndex;

// ISubstringIndex's contract, run over every structure that implements it. The contract suite is
// one class per capability - this one, CountingSubstringIndexTests, LocatingSubstringIndexTests and
// LongestRepeatIndexTests - and each asks every text in SubstringIndexTexts, random and
// adversarial, about every pattern SubstringPatterns draws from it, holding each answer to a scan
// of the text.
public sealed partial class SubstringIndexTests
{
    private static readonly Dictionary<string, Func<string, ISubstringIndex>> Indexes = new()
    {
        ["suffix array"] = text => new SuffixArrayIndex(text),
        ["suffix tree"] = text => new SuffixTreeStructure(text),
        ["suffix automaton"] = text => new SuffixAutomatonStructure(text),
    };

    public static TheoryData<string, string> Cases =>
        SubstringIndexTexts.EveryTextFor(Indexes.Keys);

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasSubstring_EveryPattern_HoldsExactlyWhenAScanFindsIt(
        string index, string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var built = Indexes[index](text);

        var wrong = SubstringPatterns.PatternsFor(text)
            .Where(pattern => built.HasSubstring(pattern) != IsFoundByScan(text, pattern))
            .ToList();

        Assert.Empty(wrong);
    }

    private static bool IsFoundByScan(string text, string pattern) =>
        BruteForceSubstrings.Starts(text, pattern).Count > 0;
}
