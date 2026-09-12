using System.Text;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.SortCharactersByFrequency;

// LeetCode 451. Sort Characters By Frequency: rearrange s so characters appear in
// non-increasing order of how often they occur (any tie-break order is valid).
//
// The baseline counts with the BCL's own Dictionary and sorts every distinct
// character by frequency. The composed strategy counts with this repo's own
// HashMap<char, int>, then pushes every (char, frequency) pair into this repo's
// own Heap<T, TOrder> ordered by ByPriorityOrder<TNode, TWeight> - the same
// (node, priority) projection TopKFrequentElementsSolution uses for LeetCode 347 -
// with the frequency negated so the heap's only order (ascending priority) pops
// characters back out in descending-frequency order, popped to completion instead
// of capped at k.
internal static class SortCharactersByFrequencySolution
{
    // The textbook approach: count with a BCL Dictionary, then sort every
    // distinct character by frequency. Deliberately written without this repo's
    // primitives - the arm the heap strategy below has to justify itself against.
    public static string FrequencySortByDictionaryOrderBy(string s)
    {
        var counts = new Dictionary<char, int>();

        foreach (var c in s)
        {
            counts[c] = counts.GetValueOrDefault(c) + 1;
        }

        var result = new StringBuilder(s.Length);

        foreach (var entry in counts.OrderByDescending(entry => entry.Value))
        {
            result.Append(entry.Key, entry.Value);
        }

        return result.ToString();
    }

    public static string FrequencySortByHashMapHeap(string s)
    {
        var counts = CountCharacters(s);
        var heap = BuildDescendingHeap(counts);

        return DrainHeapToString(heap, s.Length);
    }

    private static HashMap<char, int> CountCharacters(string s)
    {
        var counts = new HashMap<char, int>();

        foreach (var c in s)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        return counts;
    }

    private static Heap<(char Node, int Priority), ByPriorityOrder<char, int>> BuildDescendingHeap(HashMap<char, int> counts)
    {
        var heap = new Heap<(char Node, int Priority), ByPriorityOrder<char, int>>();

        foreach (var c in counts.Keys)
        {
            counts.TryGetValue(c, out var frequency);
            heap.Push((c, -frequency));
        }

        return heap;
    }

    private static string DrainHeapToString(Heap<(char Node, int Priority), ByPriorityOrder<char, int>> heap, int capacity)
    {
        var result = new StringBuilder(capacity);

        while (heap.TryPop(out var top))
        {
            result.Append(top.Node, -top.Priority);
        }

        return result.ToString();
    }
}
