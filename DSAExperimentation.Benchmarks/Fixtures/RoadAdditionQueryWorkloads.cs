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

        Carve(0, n - 1, approximateCount, queries, random);

        return [.. queries.Select(query => new[] { query.U, query.V })];
    }

    // Randomly chooses, at each still-splittable range, between carving off two
    // disjoint siblings (breadth) or emitting this whole range as one query and
    // recursing strictly inside it (nesting) - either choice keeps every emitted query
    // non-crossing with every other one, by construction.
    private static void Carve(int lo, int hi, int remaining, List<(int, int)> queries, Random random)
    {
        if (remaining <= 0 || hi - lo < 2)
        {
            return;
        }

        if (hi - lo >= 4 && random.Next(2) == 0)
        {
            var mid = random.Next(lo + 2, hi - 1);
            var half = remaining / 2;
            Carve(lo, mid, half, queries, random);
            Carve(mid, hi, remaining - half, queries, random);
            return;
        }

        queries.Add((lo, hi));
        Carve(lo + 1, hi - 1, remaining - 1, queries, random);
    }
}
