using System.Text;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortCharactersByFrequency;

// LeetCode 451. Sort Characters By Frequency: a HashMap<char,int> counts each
// character's occurrences, then every (char, frequency) pair is pushed into this
// repo's own Heap<T,TOrder> ordered by ByPriorityOrder<TNode,TWeight> - the same
// (node, priority) projection TopKFrequentElementsTests uses for LeetCode 347 -
// with the frequency negated so the heap's only order (ascending priority) pops
// characters back out in descending-frequency order, popped to completion instead
// of capped at k. Each popped character is appended its frequency times to build
// the result string.
public sealed partial class SortCharactersByFrequencyTests
{
    [Fact]
    public void FrequencySort_ClassicExample_OrdersCharactersByDescendingFrequency()
    {
        var result = FrequencySort("tree");

        Assert.True(result is "eert" or "eetr");
    }

    [Fact]
    public void FrequencySort_AllSameFrequency_PreservesEveryCharacterOccurrence()
        => AssertSameMultiset("cccaaa", FrequencySort("cccaaa"));

    [Fact]
    public void FrequencySort_SingleCharacter_ReturnsSameString()
        => Assert.Equal("a", FrequencySort("a"));

    private static string FrequencySort(string s)
    {
        var counts = new HashMap<char, int>();

        foreach (var c in s)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        var heap = new Heap<(char Node, int Priority), ByPriorityOrder<char, int>>();

        foreach (var c in counts.Keys)
        {
            counts.TryGetValue(c, out var frequency);
            heap.Push((c, -frequency));
        }

        var result = new StringBuilder(s.Length);

        while (heap.TryPop(out var top))
        {
            result.Append(top.Node, -top.Priority);
        }

        return result.ToString();
    }

    private static void AssertSameMultiset(string expected, string actual)
    {
        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected.OrderBy(c => c), actual.OrderBy(c => c));
    }
}
