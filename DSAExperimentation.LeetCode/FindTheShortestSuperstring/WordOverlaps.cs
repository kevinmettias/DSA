namespace DSAExperimentation.LeetCode.FindTheShortestSuperstring;

// LC 943's preprocessing step, and the prepared input both strategies take.
//
// overlap[i, j] is the longest suffix of word i that is also a prefix of word j -
// how much of word j can be dropped when it is appended immediately after word i.
// Once that matrix exists, "shortest superstring" is purely a word-ordering
// question, which is why both strategies below are handed one of these rather than
// recomputing string overlaps each time.
//
// This lives beside the solution rather than under Domain/ because it is meaningless
// outside this problem: it fixes LC 943's own "no word is a substring of another"
// premise and its assemble-by-dropping-the-overlap rule, the same reason
// CountWaysToBuildRoomsInAnAntColony keeps its two algebras in its own folder.
internal sealed class WordOverlaps
{
    private readonly string[] _words;
    private readonly int[,] _overlap;

    private WordOverlaps(string[] words, int[,] overlap)
    {
        _words = words;
        _overlap = overlap;
    }

    public int Count => _words.Length;

    public static WordOverlaps Build(IReadOnlyList<string> words)
    {
        var count = words.Count;
        var overlap = new int[count, count];

        for (var left = 0; left < count; left++)
        {
            for (var right = 0; right < count; right++)
            {
                overlap[left, right] = left == right ? 0 : OverlapLength(words[left], words[right]);
            }
        }

        return new WordOverlaps([.. words], overlap);
    }

    // How many characters of the word at `right` are already spelled by the tail of
    // the word at `left`.
    public int Between(int left, int right) => _overlap[left, right];

    // Concatenate the words in the given order, dropping from each the prefix its
    // predecessor already ends with. This is the answer LeetCode asks for.
    public string Assemble(int[] order)
    {
        if (order.Length == 0)
        {
            return string.Empty;
        }

        var superstring = _words[order[0]];

        for (var i = 1; i < order.Length; i++)
        {
            superstring += _words[order[i]][_overlap[order[i - 1], order[i]]..];
        }

        return superstring;
    }

    private static int OverlapLength(string left, string right)
    {
        var longest = Math.Min(left.Length, right.Length);

        for (var length = longest; length > 0; length--)
        {
            if (left.AsSpan(left.Length - length).SequenceEqual(right.AsSpan(0, length)))
            {
                return length;
            }
        }

        return 0;
    }
}
