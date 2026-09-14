using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MergeSimilarItems;

// LeetCode 2363. Merge Similar Items: two arrays of [value, weight] pairs, each
// with distinct values within itself, merged so a value appearing in both carries
// the sum of its two weights - reported ascending by value.
//
// The two strategies differ only in how they accumulate. MergeByLinearScan is the
// textbook answer, re-scanning the output list for a matching value on every
// insert (O(n) per item, O(n^2) overall) before sorting it. MergeByHashMapAndMergeSort
// accumulates into this repo's own HashMap<int, int> - the same accumulate-by-key
// convention TwoSum's map uses, summing instead of recording an index - and orders
// the distinct values with this repo's own MergeSort over an
// ArrayIndexedSequence<int>.
internal static class MergeSimilarItemsSolution
{
    // LeetCode gives each pair as a two-element array: [value, weight].
    private const int ValueIndex = 0;
    private const int WeightIndex = 1;

    // The textbook answer: one output list, scanned linearly for a matching value
    // on every insert, then sorted by value at the end. Deliberately written with a
    // BCL List and its own Sort - it is the arm the composed strategy below has to
    // justify itself against.
    public static List<(int Value, int Weight)> MergeByLinearScan(int[][] items1, int[][] items2)
    {
        var merged = new List<(int Value, int Weight)>();
        MergeInto(merged, items1);
        MergeInto(merged, items2);
        merged.Sort((left, right) => left.Value.CompareTo(right.Value));

        return merged;
    }

    private static void MergeInto(List<(int Value, int Weight)> merged, int[][] items)
    {
        foreach (var item in items)
        {
            if (!TryAddWeight(merged, item))
            {
                merged.Add((item[ValueIndex], item[WeightIndex]));
            }
        }
    }

    // The scan that makes this arm quadratic: every insert walks the whole output
    // list looking for a value already recorded.
    private static bool TryAddWeight(List<(int Value, int Weight)> merged, int[] item)
    {
        for (var index = 0; index < merged.Count; index++)
        {
            if (merged[index].Value == item[ValueIndex])
            {
                merged[index] = (merged[index].Value, merged[index].Weight + item[WeightIndex]);

                return true;
            }
        }

        return false;
    }

    // Accumulate every weight against its value in one O(1)-average pass per item,
    // then sort only the distinct values.
    public static List<(int Value, int Weight)> MergeByHashMapAndMergeSort(int[][] items1, int[][] items2)
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
            totals.TryGetValue(item[ValueIndex], out var weight);
            totals.Set(item[ValueIndex], weight + item[WeightIndex]);
        }
    }
}
