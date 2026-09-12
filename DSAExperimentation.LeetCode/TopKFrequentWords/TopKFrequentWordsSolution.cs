using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.TopKFrequentWords;

// LeetCode 692. Top K Frequent Words: count each word's occurrences, then keep the
// k "best" words under LeetCode's tie-break rule - higher frequency first, and on
// a frequency tie the lexicographically SMALLER word first.
//
// WordPriority's own IComparable orders by frequency ascending and, on a frequency
// tie, by word DESCENDING, so a size-k min-heap's root (evicted first whenever the
// heap grows past k) is always the least-frequent word, or on a tie the
// lexicographically largest one - leaving the lexicographically smallest survivor
// for equal frequencies, exactly LeetCode 692's required order. It is meaningless
// outside this problem's own tie-break rule, so it stays here rather than in
// Algorithms/ or Domain/.
internal static class TopKFrequentWordsSolution
{
    // The textbook answer: BCL Dictionary + a full sort with an explicit tie-break
    // comparer. Deliberately written without this repo's primitives - it is the
    // arm the composed solution below has to justify itself against.
    public static string[] TopKFrequentByFullSort(IReadOnlyList<string> words, int k)
    {
        var counts = new Dictionary<string, int>();

        foreach (var word in words)
        {
            counts[word] = counts.GetValueOrDefault(word) + 1;
        }

        return counts
            .OrderByDescending(entry => entry.Value)
            .ThenBy(entry => entry.Key, StringComparer.Ordinal)
            .Take(k)
            .Select(entry => entry.Key)
            .ToArray();
    }

    // This repo's own HashMap<string,int> counts occurrences, then a size-k
    // Heap<Element,TOrder> closed over MinHeapOrder<WordPriority> keeps only the k
    // "best" words - the same size-k-heap shape TopKFrequentElements (LC 347) uses,
    // closed over a single IComparable key instead of ByPriorityOrder<TNode,TWeight>
    // so the frequency-descending primary order and the word-ascending tie-break
    // both live in one comparison.
    public static string[] TopKFrequentByMinHeap(IReadOnlyList<string> words, int k)
    {
        var counts = CountFrequencies(words);
        var heap = BuildTopKHeap(counts, k);

        return ExtractOrderedResult(heap);
    }

    private static HashMap<string, int> CountFrequencies(IReadOnlyList<string> words)
    {
        var counts = new HashMap<string, int>();

        foreach (var word in words)
        {
            counts.TryGetValue(word, out var count);
            counts.Set(word, count + 1);
        }

        return counts;
    }

    private static Heap<WordPriority, MinHeapOrder<WordPriority>> BuildTopKHeap(HashMap<string, int> counts, int k)
    {
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

        return heap;
    }

    private static string[] ExtractOrderedResult(Heap<WordPriority, MinHeapOrder<WordPriority>> heap)
    {
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
