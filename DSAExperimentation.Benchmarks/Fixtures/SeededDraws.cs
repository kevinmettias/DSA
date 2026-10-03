namespace DSAExperimentation.Benchmarks.Fixtures;

// The two seeded draws most workloads are built from: an array of values in
// [lowInclusive, highExclusive), and a batch of two-element query pairs over a
// range. Every benchmark and fixture that draws a plain array of values draws it
// here rather than restating the Enumerable.Range/Select/Next pipeline, so a
// workload's shape is read off one call. Pairs is the shape LC 2940's meeting
// queries, LC 1707's limit queries and LC 3569's split queries share, where values
// come from a range far wider than their count so a query's bounds genuinely
// partition them.
//
// A Random is taken rather than a seed because a caller interleaves these draws: one
// stream feeds the values and then the queries, and a fresh Random per call would
// restart it, correlating the queries with the values and changing the workload a
// recorded measurement was taken against.
internal static class SeededDraws
{
    public static int[] Values(int count, int lowInclusive, int highExclusive, Random random) =>
        Enumerable.Range(0, count).Select(_ => random.Next(lowInclusive, highExclusive)).ToArray();

    // Values with every repeat skipped and drawn again, for a problem that promises its values
    // distinct - or builds pairs and triples from them, where a repeat would put one value in the
    // array four or six times. Up to the first repeat the draws are exactly Values', so a workload
    // that never drew one is unchanged. The range must hold count values.
    public static int[] DistinctValues(int count, int lowInclusive, int highExclusive, Random random)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, highExclusive - lowInclusive);

        var drawn = new HashSet<int>(count);
        var values = new List<int>(count);

        // Ends once count distinct values are in hand, which the range check above makes reachable.
        while (values.Count < count)
        {
            var value = random.Next(lowInclusive, highExclusive);
            var isFirstDraw = drawn.Add(value);

            if (isFirstDraw)
            {
                values.Add(value);
            }
        }

        return [.. values];
    }

    // Two independent draws of the same quantity, which is the shape of LC 2940's
    // pair of building indices and LC 1707's (xor operand, limit) pair.
    public static int[][] Pairs(int count, int lowInclusive, int highExclusive, Random random) =>
        Enumerable.Range(0, count)
            .Select(_ => new[] { random.Next(lowInclusive, highExclusive), random.Next(lowInclusive, highExclusive) })
            .ToArray();
}
