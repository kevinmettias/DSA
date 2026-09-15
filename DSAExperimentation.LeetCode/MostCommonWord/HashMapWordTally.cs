using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MostCommonWord;

// This repo's own HashMap<string,int> tallies occurrences and Set<string> answers
// "banned" in O(1), both built from the problem's own inputs - the same
// count-with-HashMap shape TopKFrequentWords uses for LC 692.
internal sealed class HashMapWordTally : IWordTally
{
    private readonly Set<string> _bannedWords = new();
    private readonly HashMap<string, int> _counts = new();

    // The banned list is lower-cased once, here, rather than at every lookup: every
    // token arrives lower-cased from the tokenizer, so a case-folded list is the only
    // form a token can be matched against.
    public HashMapWordTally(string[] banned)
    {
        foreach (var word in banned)
        {
            _bannedWords.TryAdd(word.ToLowerInvariant());
        }
    }

    public bool IsBanned(string word) => _bannedWords.Has(word);

    // This word's new tally in the count map: the stored count, one higher, written
    // back.
    public int Count(string word)
    {
        _counts.TryGetValue(word, out var count);
        count++;
        _counts.Set(word, count);

        return count;
    }
}
