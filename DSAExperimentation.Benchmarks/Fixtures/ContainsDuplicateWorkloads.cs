namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 217 - a shuffled 0..length-1 range keeps every
// value distinct without relying on random collisions to avoid them, so both
// strategies pay the full worst case (no duplicate to find early) instead of
// stopping as soon as a lucky repeat turns up.
internal static class ContainsDuplicateWorkloads
{
    public static int[] BuildDistinctValues(int length, int seed)
    {
        var random = new Random(seed);
        var values = new int[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = i;
        }

        for (var i = length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }
}
