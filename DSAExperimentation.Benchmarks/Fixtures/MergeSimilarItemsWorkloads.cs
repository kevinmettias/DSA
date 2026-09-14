namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 2363: items1 holds even values, items2 holds odd ones, so
// no value ever repeats across the two arrays and neither arm gets an early-exit
// shortcut from a shared value - the naive linear scan always pays its full-length
// walk. Every weight is 1, because which weights are summed is irrelevant to the
// cost of finding them.
internal static class MergeSimilarItemsWorkloads
{
    private const int Weight = 1;

    // Values are laid out with a stride of two so items1 can take the evens and
    // items2 the odds - two disjoint value sets of the requested length.
    private const int Stride = 2;

    public static (int[][] Items1, int[][] Items2) BuildDisjointValues(int length) =>
        (BuildPairs(length, firstValue: 0), BuildPairs(length, firstValue: 1));

    private static int[][] BuildPairs(int length, int firstValue)
    {
        var items = new int[length][];

        for (var i = 0; i < length; i++)
        {
            items[i] = [firstValue + (i * Stride), Weight];
        }

        return items;
    }
}
