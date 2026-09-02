namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3609 - an arbitrary (tx,ty) is reachable from a
// given start only along this exact move rule (almost never otherwise), so a valid
// pair is built by replaying `moveCount` random forward moves from a small, fixed
// start rather than picking coordinates directly.
internal static class TargetGridWorkloads
{
    private const int StartX = 1;
    private const int StartY = 1;

    public static (int Sx, int Sy, int Tx, int Ty) BuildReachablePair(int moveCount, int seed)
    {
        var random = new Random(seed);
        long x = StartX;
        long y = StartY;

        for (var i = 0; i < moveCount; i++)
        {
            var m = Math.Max(x, y);

            if (random.Next(2) == 0)
            {
                x += m;
            }
            else
            {
                y += m;
            }
        }

        return (StartX, StartY, (int)x, (int)y);
    }
}
