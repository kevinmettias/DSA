using System.Text;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.MostCommonWord;

// LeetCode 819. Most Common Word: the most frequent lower-cased word in a
// paragraph that is not on the banned list, where words are separated by spaces
// and/or punctuation and the answer is guaranteed unique.
//
// Both strategies walk the same tokens and keep a running best as they count, so
// the only thing they differ in is which pair of structures answers "have I seen
// this word before" and "is this word banned" - a BCL Dictionary + HashSet, or
// this repo's own HashMap<string,int> (word counts) plus Set<string> (words to
// skip entirely), the same count-with-HashMap shape TopKFrequentWords uses for
// LC 692.
internal static class MostCommonWordSolution
{
    // The textbook answer: BCL Dictionary<string,int> for the tally and
    // HashSet<string> for the banned list. Deliberately written without this
    // repo's primitives - it is the arm the composed strategy below has to
    // justify itself against.
    public static string MostCommonByDictionaryScan(string paragraph, string[] banned)
    {
        var words = Tokenize(paragraph);

        return MostCommonByDictionaryScan(words, banned);
    }

    // Tokenization hoisted out, so a benchmark charges it to [GlobalSetup] rather
    // than to the counting scan being measured.
    public static string MostCommonByDictionaryScan(DynamicArray<string> words, string[] banned)
    {
        var tally = new DictionaryWordTally(banned);

        return MostCommonOf(words, tally);
    }

    // This repo's own HashMap<string,int> tallies occurrences and Set<string>
    // answers "banned" in O(1), with the running best updated in the same pass so
    // no second sweep over the tally is needed.
    public static string MostCommonByHashMapTally(string paragraph, string[] banned)
    {
        var words = Tokenize(paragraph);

        return MostCommonByHashMapTally(words, banned);
    }

    public static string MostCommonByHashMapTally(DynamicArray<string> words, string[] banned)
    {
        var tally = new HashMapWordTally(banned);

        return MostCommonOf(words, tally);
    }

    // The counting pass both strategies share: walk the tokens once, skip the banned
    // ones, and let each surviving word challenge the running winner as its tally
    // lands. Which pair of structures answers "is this banned" and "what is this
    // word's new tally" is the arms' only disagreement, so it arrives as a named
    // strategy type - both questions, both inputs, and the contract behind them are
    // written down on IWordTally rather than left as two bare delegates.
    private static string MostCommonOf(DynamicArray<string> words, IWordTally tally)
    {
        var best = new BestWord(string.Empty, 0);

        for (var i = 0; i < words.Count; i++)
        {
            var word = words.Get(i);

            if (tally.IsBanned(word))
            {
                continue;
            }

            var count = tally.Count(word);
            best = best.Challenge(word, count);
        }

        return best.Word;
    }

    // Splits on runs of non-letters - LeetCode's own "words are separated by
    // spaces and/or punctuation" definition - lower-casing as it goes, into this
    // repo's own DynamicArray so the prepared-token overloads above take a
    // container the paragraph overloads can never bind to.
    private static DynamicArray<string> Tokenize(string paragraph)
    {
        var words = new DynamicArray<string>();
        var current = new StringBuilder();

        foreach (var character in paragraph)
        {
            if (char.IsLetter(character))
            {
                current.Append(char.ToLowerInvariant(character));
                continue;
            }

            FlushCurrent(current, words);
            current.Clear();
        }

        FlushCurrent(current, words);

        return words;
    }

    private static void FlushCurrent(StringBuilder current, DynamicArray<string> words)
    {
        if (current.Length > 0)
        {
            words.Add(current.ToString());
        }
    }

    // The running winner. LeetCode guarantees exactly one answer, so a strict
    // improvement is the only thing that can displace the current best.
    private readonly record struct BestWord(string Word, int Count)
    {
        public BestWord Challenge(string word, int count) =>
            count > Count ? new BestWord(word, count) : this;
    }
}
