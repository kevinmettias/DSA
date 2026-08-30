using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueriesOnAPermutationWithKey;

// LeetCode 1409. Queries on a Permutation With Key: simulate the permutation P
// as this repo's own DynamicArray<int> - a linear scan via Get finds the
// queried value's current index, RemoveAt/Insert(0, ...) then moves it to the
// front, the same DynamicArray-as-shrinking/growing-sequence idiom
// PermutationSequenceTests already uses for a different (factoradic) purpose.
public sealed partial class QueriesOnAPermutationWithKeyTests
{
    [Fact]
    public void ProcessQueries_LeetCodeExampleOne_ReturnsQueriedIndices()
    {
        int[] queries = [3, 1, 2, 1];

        var result = ProcessQueries(queries, m: 5);

        Assert.Equal([2, 1, 2, 1], result);
    }

    [Fact]
    public void ProcessQueries_LeetCodeExampleTwo_ReturnsQueriedIndices()
    {
        int[] queries = [4, 1, 2, 2];

        var result = ProcessQueries(queries, m: 4);

        Assert.Equal([3, 1, 2, 0], result);
    }

    private static List<int> ProcessQueries(int[] queries, int m)
    {
        var permutation = new DynamicArray<int>();
        for (var value = 1; value <= m; value++)
        {
            permutation.Add(value);
        }

        var result = new List<int>();

        foreach (var query in queries)
        {
            var index = IndexOf(permutation, query);
            result.Add(index);
            permutation.RemoveAt(index);
            permutation.Insert(0, query);
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

        return -1;
    }
}
