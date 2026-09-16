using DSAExperimentation.DataStructures.HashMap;
using WaitingQueue = DSAExperimentation.DataStructures.Queue.Queue<(string Word, int Index)>;

namespace DSAExperimentation.LeetCode.NumberOfMatchingSubsequences;

// LeetCode 792. Number of Matching Subsequences: how many of words are
// subsequences of searchedText.
//
// CountMatchingSubseqByTwoPointerPerWord is the natural approach - one two-pointer
// scan through searchedText per word, O(words * searchedText.Length).
// CountMatchingSubseqByWaitingBuckets is this repo's composition: every word waits
// in a bucket keyed by the next character it needs (a HashMap<char, Queue<T>>), so a
// single left-to-right pass over searchedText advances every waiting word in step
// instead of re-scanning searchedText once per word -
// O(searchedText.Length + total word length).
internal static class NumberOfMatchingSubsequencesSolution
{
    // The textbook answer: check each word independently with a two-pointer walk.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int CountMatchingSubseqByTwoPointerPerWord(string searchedText, string[] words)
    {
        var matches = 0;

        foreach (var word in words)
        {
            if (IsSubsequence(new CandidateWord(word), new SearchedText(searchedText)))
            {
                matches++;
            }
        }

        return matches;
    }

    private static bool IsSubsequence(CandidateWord word, SearchedText searchedText)
    {
        var next = 0;

        for (var i = 0; i < searchedText.Text.Length && next < word.Text.Length; i++)
        {
            if (searchedText.Text[i] == word.Text[next])
            {
                next++;
            }
        }

        return next == word.Text.Length;
    }

    // One pass over searchedText: reaching character symbol releases exactly the words
    // waiting on symbol, each of which either finishes or re-queues under the next
    // character it needs. HashMap<char, Queue<T>> is the whole data structure.
    public static int CountMatchingSubseqByWaitingBuckets(string searchedText, string[] words)
    {
        var buckets = new HashMap<char, WaitingQueue>();
        SeedBuckets(buckets, words);

        var matches = 0;

        foreach (var symbol in searchedText)
        {
            if (buckets.TryGetValue(symbol, out var waiting))
            {
                matches += AdvanceBucket(buckets, waiting);
            }
        }

        return matches;
    }

    private static void SeedBuckets(HashMap<char, WaitingQueue> buckets, string[] words)
    {
        foreach (var word in words)
        {
            Enqueue(buckets, word, 0);
        }
    }

    // Only the words waiting when this character was reached are advanced: a word
    // re-queued onto the same bucket is waiting on a LATER occurrence of the
    // character, so the pending count is snapshotted before the loop.
    private static int AdvanceBucket(HashMap<char, WaitingQueue> buckets, WaitingQueue waiting)
    {
        var pending = waiting.Count;
        var matches = 0;

        for (var i = 0; i < pending; i++)
        {
            waiting.TryDequeue(out var entry);
            var nextIndex = entry.Index + 1;

            if (nextIndex == entry.Word.Length)
            {
                matches++;
            }
            else
            {
                Enqueue(buckets, entry.Word, nextIndex);
            }
        }

        return matches;
    }

    private static void Enqueue(HashMap<char, WaitingQueue> buckets, string word, int index)
    {
        var symbol = word[index];

        if (!buckets.TryGetValue(symbol, out var waiting))
        {
            waiting = new WaitingQueue();
            buckets.Set(symbol, waiting);
        }

        waiting.Enqueue((word, index));
    }

    // LC 792's two operands, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `word` is the candidate whose characters have to appear
    // in order and `searchedText` the string they are looked for in - subsequence
    // containment is one-directional, so a swap asks a different question.
    internal readonly record struct CandidateWord(string Text);

    internal readonly record struct SearchedText(string Text);
}
