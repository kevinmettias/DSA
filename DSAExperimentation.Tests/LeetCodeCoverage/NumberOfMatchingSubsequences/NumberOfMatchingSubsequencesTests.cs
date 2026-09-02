using DSAExperimentation.DataStructures.HashMap;
using WaitingQueue = DSAExperimentation.DataStructures.Queue.Queue<(string Word, int Index)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfMatchingSubsequences;

// LeetCode 792. Number of Matching Subsequences: bucket each word by the next
// character it's waiting on, using this repo's own HashMap<TKey,TValue> keyed on
// char plus a Queue<T> per bucket. A single left-to-right pass over s advances
// every waiting word in step, instead of re-scanning s once per word.
public sealed partial class NumberOfMatchingSubsequencesTests
{
    [Fact]
    public void NumMatchingSubseq_ClassicExample_CountsMatchingWords()
    {
        var count = NumMatchingSubseq("abcde", ["a", "bb", "acd", "ace"]);

        Assert.Equal(3, count);
    }

    [Fact]
    public void NumMatchingSubseq_NoWordsMatch_ReturnsZero()
    {
        var count = NumMatchingSubseq("abc", ["xyz", "ba"]);

        Assert.Equal(0, count);
    }

    private static int NumMatchingSubseq(string s, string[] words)
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
