namespace DSAExperimentation.LeetCode.MostCommonWord;

// The textbook arm's pair: a BCL Dictionary<string,int> for the tally and a
// HashSet<string> for the banned list, both built from the problem's own inputs.
// Deliberately written without this repo's primitives - it is the arm the composed
// strategy has to justify itself against.
internal sealed class DictionaryWordTally : IWordTally
{
    private readonly HashSet<string> _bannedWords = new();
    private readonly Dictionary<string, int> _counts = new();

    // The banned list is lower-cased once, here, rather than at every lookup: every
    // token arrives lower-cased from the tokenizer, so a case-folded list is the only
    // form a token can be matched against.
    public DictionaryWordTally(string[] banned)
    {
        foreach (var word in banned)
        {
            _bannedWords.Add(word.ToLowerInvariant());
        }
    }

    public bool IsBanned(string word) => _bannedWords.Contains(word);

    // This word's new tally in the BCL count map: the stored count, one higher, written
    // back.
    public int Count(string word)
    {
        var count = _counts.GetValueOrDefault(word) + 1;
        _counts[word] = count;

        return count;
    }
}
