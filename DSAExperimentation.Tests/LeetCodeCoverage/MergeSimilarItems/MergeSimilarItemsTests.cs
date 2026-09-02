using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeSimilarItems;

// LeetCode 2363. Merge Similar Items: a HashMap<int,int> sums each value's weight
// across both item lists - the same accumulate-by-key convention TwoSumTests'
// HashMap<TValue,TIndex> use, just summing instead of recording an index - then
// MergeSort over the distinct values (ArrayIndexedSequence<int>, the same
// "sort with this repo's MergeSort" convention AccountsMergeTests/SortAnArrayTests
// already use) produces the ascending-by-value order LeetCode expects.
public sealed partial class MergeSimilarItemsTests
{
    [Fact]
    public void Merge_ClassicExample_SumsOverlappingValuesAndKeepsUniqueOnes()
    {
        int[][] items1 = [[1, 1], [4, 5], [3, 8]];
        int[][] items2 = [[3, 1], [1, 5]];

        var merged = MergeItems(items1, items2);

        Assert.Equal([(1, 6), (3, 9), (4, 5)], merged);
    }

    [Fact]
    public void Merge_EveryValueOverlaps_SumsAllThreeValues()
    {
        int[][] items1 = [[1, 1], [3, 2], [2, 3]];
        int[][] items2 = [[2, 1], [3, 2], [1, 3]];

        var merged = MergeItems(items1, items2);

        Assert.Equal([(1, 4), (2, 4), (3, 4)], merged);
    }

    [Fact]
    public void Merge_NoOverlap_KeepsEachValueSeparateAndSorted()
    {
        int[][] items1 = [[1, 3], [2, 2]];
        int[][] items2 = [[7, 1], [2, 2], [1, 4]];

        var merged = MergeItems(items1, items2);

        Assert.Equal([(1, 7), (2, 4), (7, 1)], merged);
    }

    private static List<(int Value, int Weight)> MergeItems(int[][] items1, int[][] items2)
    {
        var totals = new HashMap<int, int>();
        Accumulate(totals, items1);
        Accumulate(totals, items2);

        var values = totals.Keys.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(values));

        var merged = new List<(int Value, int Weight)>(values.Length);

        foreach (var value in values)
        {
            totals.TryGetValue(value, out var weight);
            merged.Add((value, weight));
        }

        return merged;
    }

    private static void Accumulate(HashMap<int, int> totals, int[][] items)
    {
        foreach (var item in items)
        {
            totals.TryGetValue(item[0], out var weight);
            totals.Set(item[0], weight + item[1]);
        }
    }
}
