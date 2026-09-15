namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3244 - queries are carved out of [0, n-1] so that
// any two are either nested or disjoint by construction, satisfying the "II" variant's
// own non-crossing guarantee rather than filtering random pairs down to the ones that
// happen to qualify.
internal static class RoadAdditionQueryWorkloads
{
    public static int[][] BuildQueries(int n, int approximateCount, int seed)
    {
        var random = new Random(seed);
        var queries = new List<(int U, int V)>();

        Carve((0, n - 1), approximateCount, queries, random);

        return [.. queries.Select(query => new[] { query.U, query.V })];
    }

    // Randomly chooses, at each still-splittable range, between carving off two
    // disjoint siblings (breadth) or emitting this whole range as one query and
    // recursing strictly inside it (nesting) - either choice keeps every emitted query
    // non-crossing with every other one, by construction. The two endpoints are one
    // range, so they are one argument wherever the recursion carries them.
    private static void Carve((int Lo, int Hi) range, int remaining, List<(int, int)> queries, Random random)
    {
        if (remaining <= 0 || range.Hi - range.Lo < 2)
        {
            return;
        }

        if (range.Hi - range.Lo >= 4 && random.Next(2) == 0)
        {
            var mid = random.Next(range.Lo + 2, range.Hi - 1);
            var half = remaining / 2;
            Carve((range.Lo, mid), half, queries, random);
            Carve((mid, range.Hi), remaining - half, queries, random);
            return;
        }

        queries.Add((range.Lo, range.Hi));
        Carve((range.Lo + 1, range.Hi - 1), remaining - 1, queries, random);
    }
}
