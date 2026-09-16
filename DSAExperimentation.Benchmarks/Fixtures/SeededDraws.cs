namespace DSAExperimentation.Benchmarks.Fixtures;

// The two seeded draws three offline-sweep harnesses need: an array of values in
// [lowInclusive, highExclusive), and a batch of two-element query pairs over a
// range. Values normally come from a range far wider than the value count, so a
// query's bounds genuinely partition them instead of selecting everything - the
// workload LC 2940's meeting queries, LC 1707's limit queries and LC 3569's split
// queries all share.
//
// A Random is taken rather than a seed because a caller interleaves these draws: one
// stream feeds the values and then the queries, and a fresh Random per call would
// restart it, correlating the queries with the values and changing the workload a
// recorded measurement was taken against.
internal static class SeededDraws
{
    public static int[] Values(int count, int lowInclusive, int highExclusive, Random random) =>
        Enumerable.Range(0, count).Select(_ => random.Next(lowInclusive, highExclusive)).ToArray();

    // Two independent draws of the same quantity, which is the shape of LC 2940's
    // pair of building indices and LC 1707's (xor operand, limit) pair.
    public static int[][] Pairs(int count, int lowInclusive, int highExclusive, Random random) =>
        Enumerable.Range(0, count)
            .Select(_ => new[] { random.Next(lowInclusive, highExclusive), random.Next(lowInclusive, highExclusive) })
            .ToArray();
}
