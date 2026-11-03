namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for the two minimum-area-rectangle siblings - LC 939
// (axis-aligned) and LC 963 (free rectangles, rotations included) - whose inputs are
// likewise the same: distinct lattice points drawn from a grid barely larger than the
// point count, so rectangles are plentiful and either problem's arms do real
// corner-confirmation work instead of scanning a sparse set where nothing lines up.
internal static class LatticePointWorkloads
{
    // The padding widens the grid past ceil(sqrt(count)) so the rejection loop below
    // terminates promptly while the grid stays close to the point count; each problem
    // picks its own, since LC 963's quadruple scan needs the smaller set.
    public static int[][] InGrid(int count, int gridPadding, int seed)
    {
        var random = new Random(seed);
        var grid = (int)Math.Ceiling(Math.Sqrt(count)) + gridPadding;
        // Kept in the order drawn, the set only checking membership: a HashSet's order follows the
        // points' hashes, which .NET seeds afresh in every process, so taking its order would give each
        // benchmark process a different workload from the same seed.
        var seen = new HashSet<(int X, int Y)>();
        var drawn = new List<int[]>(count);

        while (drawn.Count < count)
        {
            var point = (X: random.Next(grid), Y: random.Next(grid));
            var isNewPoint = seen.Add(point);

            if (isNewPoint)
            {
                drawn.Add([point.X, point.Y]);
            }
        }

        return [.. drawn];
    }
}
