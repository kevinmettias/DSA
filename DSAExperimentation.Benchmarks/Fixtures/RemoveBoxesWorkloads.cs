namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 546 - a random scattering of box colors. Kept
// well under LeetCode's own limit (<=24 in the benchmark's own [Params], vs. LC's
// 100) because RemoveBoxesSolution's un-memoized baseline is genuinely exponential:
// a three-dimensional (left, right, extra) state space blows up faster than
// BurstBalloonsWorkloads' two-dimensional one, so that fixture's cap of 14 would
// already be too slow here.
internal static class RemoveBoxesWorkloads
{
    private const int MaxBoxColorValueExclusive = 4;

    public static int[] BuildBoxes(int count, int seed)
    {
        var random = new Random(seed);
        var boxes = new int[count];

        for (var i = 0; i < count; i++)
        {
            boxes[i] = random.Next(1, MaxBoxColorValueExclusive);
        }

        return boxes;
    }
}
