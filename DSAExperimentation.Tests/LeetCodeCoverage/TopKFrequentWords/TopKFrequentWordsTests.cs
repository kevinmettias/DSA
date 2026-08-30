using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TopKFrequentWords;

// LeetCode 692. Top K Frequent Words: a HashMap<string,int> counts each word's
// occurrences, then a size-k min-heap - this repo's own Heap<Element,TOrder>
// closed over the generic MinHeapOrder<Element> (the same size-k-heap shape
// ReconstructItineraryTests already closes MinHeapOrder over bare strings for
// LeetCode 332) - keeps only the k "best" words. WordPriority's own IComparable
// orders by frequency ascending and, on a frequency tie, by word DESCENDING, so
// the heap's root (evicted first whenever the heap grows past k) is always the
// least-frequent word, or on a tie the lexicographically largest one, leaving the
// lexicographically smallest survivor for equal frequencies - exactly LeetCode
// 692's required tie-break rule.
public sealed partial class TopKFrequentWordsTests
{
    [Fact]
    public void TopKFrequent_ClassicExample_BreaksTiesLexicographically()
    {
        string[] words = ["i", "love", "leetcode", "i", "love", "coding"];

        var result = TopKFrequent(words, k: 2);

        Assert.Equal(["i", "love"], result);
    }

    [Fact]
    public void TopKFrequent_AllDistinctFrequencies_OrdersByFrequencyDescending()
    {
        string[] words = ["the", "day", "is", "sunny", "the", "the", "the", "sunny", "is", "is"];

        var result = TopKFrequent(words, k: 4);

        Assert.Equal(["the", "is", "sunny", "day"], result);
    }

    private static string[] TopKFrequent(string[] words, int k)
    {
        var counts = new HashMap<string, int>();

        foreach (var word in words)
        {
            counts.TryGetValue(word, out var count);
            counts.Set(word, count + 1);
        }

        var heap = new Heap<WordPriority, MinHeapOrder<WordPriority>>();

        foreach (var word in counts.Keys)
        {
            counts.TryGetValue(word, out var frequency);
            heap.Push(new WordPriority(frequency, word));

            if (heap.Count > k)
            {
                heap.TryPop(out _);
            }
        }

        var result = new string[heap.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            heap.TryPop(out var top);
            result[i] = top.Word;
        }

        return result;
    }

    private readonly record struct WordPriority(int Frequency, string Word) : IComparable<WordPriority>
    {
        public int CompareTo(WordPriority other) => Frequency != other.Frequency
            ? Frequency.CompareTo(other.Frequency)
            : string.CompareOrdinal(other.Word, Word);
    }
}
