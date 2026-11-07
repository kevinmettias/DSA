using DSAExperimentation.DataStructures.SubstringIndex;
using DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;
using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;
using SuffixAutomatonStructure = DSAExperimentation.DataStructures.SuffixAutomaton.SuffixAutomaton;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SubstringIndex;

// ILongestRepeatIndex's contract over the bare suffix array, the suffix tree and the suffix
// automaton. The one-letter run's answer, 23 of its 24 letters, holds the contract to overlapping
// occurrences, and the all-distinct text's to the empty answer. Ties may be broken differently, so
// each answer is held to the longest repeat's length and to naming a substring the text holds
// twice, not to one particular occurrence.
public sealed partial class LongestRepeatIndexTests
{
    private const int RepeatOccurrences = 2;

    private static readonly Dictionary<string, Func<string, ILongestRepeatIndex>> Indexes = new()
    {
        ["suffix array"] = text => new SuffixArrayStructure(text),
        ["suffix tree"] = text => new SuffixTreeStructure(text),
        ["suffix automaton"] = text => new SuffixAutomatonStructure(text),
    };

    public static TheoryData<string, string> Cases =>
        SubstringIndexTexts.EveryTextFor(Indexes.Keys);

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLongestRepeat_EveryText_IsAsLongAsTheLongestRepeat(
        string index, string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var repeat = Indexes[index](text).FindLongestRepeat();

        Assert.Equal(BruteForceSubstrings.LongestRepeatLength(text), repeat.Length);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLongestRepeat_EveryText_PointsAtASubstringThatOccursTwice(
        string index, string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var repeat = Indexes[index](text).FindLongestRepeat();

        Assert.True(IsRepeatedOrEmpty(text, repeat), $"{repeat} in \"{text}\"");
    }

    // The empty answer is exactly SubstringOccurrence.Empty; any other names a substring the text
    // holds at least twice.
    private static bool IsRepeatedOrEmpty(string text, SubstringOccurrence repeat)
    {
        if (repeat.Length == 0)
        {
            return repeat == SubstringOccurrence.Empty;
        }

        var substring = text.Substring(repeat.Start, repeat.Length);
        return BruteForceSubstrings.Starts(text, substring).Count >= RepeatOccurrences;
    }
}
