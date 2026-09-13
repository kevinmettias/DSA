using DSAExperimentation.DataStructures.HashMap;
using WaitingQueue = DSAExperimentation.DataStructures.Queue.Queue<(string Word, int Index)>;

namespace DSAExperimentation.LeetCode.NumberOfMatchingSubsequences;

// LeetCode 792. Number of Matching Subsequences: how many of words are
// subsequences of s.
//
// NumMatchingSubseqByTwoPointerPerWord is the natural approach - one two-pointer
// scan through s per word, O(words * s.Length). NumMatchingSubseqByWaitingBuckets
// is this repo's composition: every word waits in a bucket keyed by the next
// character it needs (a HashMap<char, Queue<T>>), so a single left-to-right pass
// over s advances every waiting word in step instead of re-scanning s once per
// word - O(s.Length + total word length).
internal static class NumberOfMatchingSubsequencesSolution
{
    // The textbook answer: check each word independently with a two-pointer walk.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int NumMatchingSubseqByTwoPointerPerWord(string s, string[] words)
    {
        var matches = 0;

        foreach (var word in words)
        {
            if (IsSubsequence(word, s))
            {
                matches++;
            }
        }

        return matches;
    }

    private static bool IsSubsequence(string word, string s)
    {
        var next = 0;

        for (var i = 0; i < s.Length && next < word.Length; i++)
        {
            if (s[i] == word[next])
            {
                next++;
            }
        }

        return next == word.Length;
    }

    // One pass over s: reaching character c releases exactly the words waiting on
    // c, each of which either finishes or re-queues under the next character it
    // needs. HashMap<char, Queue<T>> is the whole data structure.
    public static int NumMatchingSubseqByWaitingBuckets(string s, string[] words)
    {
        var buckets = new HashMap<char, WaitingQueue>();
        SeedBuckets(buckets, words);

        var matches = 0;

        foreach (var c in s)
        {
            if (buckets.TryGetValue(c, out var waiting))
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
        var c = word[index];

        if (!buckets.TryGetValue(c, out var waiting))
        {
            waiting = new WaitingQueue();
            buckets.Set(c, waiting);
        }

        waiting.Enqueue((word, index));
    }
}
