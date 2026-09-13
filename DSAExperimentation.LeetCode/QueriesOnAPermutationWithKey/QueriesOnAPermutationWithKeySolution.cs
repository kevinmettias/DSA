using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.QueriesOnAPermutationWithKey;

// LeetCode 1409. Queries on a Permutation With Key: P starts as [1..m]; each
// query reports the queried value's current index in P and then moves that
// value to the front. The answer is the list of reported indices, one per
// query.
//
// Both strategies run the identical move-to-front simulation and are both
// O(Queries * M) - locating a value is a linear scan and re-inserting it at the
// front shifts everything behind it, either way. They differ only in the
// sequence container: a BCL List<int>, or this repo's own DynamicArray<int>
// used as a shrinking/growing sequence, the same idiom
// PermutationSequenceSolution's factoradic arm already leans on. So this pair
// measures the repo primitive's overhead against the BCL type doing the
// identical job, not a different complexity class.
internal static class QueriesOnAPermutationWithKeySolution
{
    private const int FirstPermutationValue = 1;
    private const int FrontIndex = 0;

    // The textbook answer: a BCL List<int> with its own IndexOf/RemoveAt/Insert.
    // Deliberately written without this repo's primitives - it is the arm the
    // DynamicArray strategy below has to justify itself against.
    public static List<int> ProcessQueriesByListMoveToFront(int[] queries, int m)
    {
        var permutation = new List<int>(m);

        for (var value = FirstPermutationValue; value <= m; value++)
        {
            permutation.Add(value);
        }

        var result = new List<int>(queries.Length);

        foreach (var query in queries)
        {
            var index = permutation.IndexOf(query);
            result.Add(index);
            permutation.RemoveAt(index);
            permutation.Insert(FrontIndex, query);
        }

        return result;
    }

    // The same simulation over this repo's own DynamicArray<int>: Get(i) drives
    // the scan, RemoveAt/Insert(0, ...) perform the move-to-front, all three
    // operations the structure already exposes.
    public static List<int> ProcessQueriesByDynamicArrayMoveToFront(int[] queries, int m)
    {
        var permutation = new DynamicArray<int>();

        for (var value = FirstPermutationValue; value <= m; value++)
        {
            permutation.Add(value);
        }

        var result = new List<int>(queries.Length);

        foreach (var query in queries)
        {
            var index = IndexOf(permutation, query);
            result.Add(index);
            permutation.RemoveAt(index);
            permutation.Insert(FrontIndex, query);
        }

        return result;
    }

    private static int IndexOf(DynamicArray<int> permutation, int value)
    {
        for (var i = 0; i < permutation.Count; i++)
        {
            if (permutation.Get(i) == value)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }
}
