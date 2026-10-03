namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 2363: items1 holds even values, items2 holds odd ones, so
// no value ever repeats across the two arrays and neither arm gets an early-exit
// shortcut from a shared value - the naive linear scan always pays its full-length
// walk. Every weight is 1, because which weights are summed is irrelevant to the
// cost of finding them. Together the two arrays hold exactly 1 through 2 * length,
// so LC 2363's 1 <= value <= 1000 holds up to a length of 500.
internal static class MergeSimilarItemsWorkloads
{
    private const int Weight = 1;

    // Values are laid out with a stride of two so items1 can take the evens and
    // items2 the odds - two disjoint value sets of the requested length.
    private const int Stride = 2;

    // LC 2363's values start at 1, so the evens start at 2.
    private const int FirstEvenValue = 2;
    private const int FirstOddValue = 1;

    public static (int[][] Items1, int[][] Items2) BuildDisjointValues(int length) =>
        (BuildPairs(length, FirstEvenValue), BuildPairs(length, FirstOddValue));

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
