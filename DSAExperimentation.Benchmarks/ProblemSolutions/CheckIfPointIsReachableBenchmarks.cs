using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(long X, long Y)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check if Point Is Reachable (LC 2543): the textbook brute force - this
// repo's own Queue<Element> + Set<Element> running a BFS over every
// intermediate point reachable from (1, 1) via the four moves (x, x + y),
// (x + y, y), (2x, y), (x, 2y), bounded above by targetX/targetY since every
// move strictly grows at least one coordinate - vs. the analytic
// Gcd-is-a-power-of-two check (CheckIfPointIsReachableTests precedent), the
// same "materialize and search the whole state space vs. one closed-form
// check" contrast TwoSumBenchmarks runs for its own brute-force-vs-hash-map
// pair. Targets are consecutive integers (gcd 1, always reachable) so BFS is
// always forced to search rather than short-circuiting on an early false.
[MemoryDiagnoser]
public class CheckIfPointIsReachableBenchmarks
{
    [Params(50, 300)]
    public int Length;

    private long _targetX;
    private long _targetY;

    [GlobalSetup]
    public void Setup()
    {
        _targetX = Length;
        _targetY = Length + 1;
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceBfs() => IsReachableByBruteForceBfs(_targetX, _targetY);

    [Benchmark]
    public bool GcdIsPowerOfTwo() => IsReachableByGcd(_targetX, _targetY);

    private static bool IsReachableByBruteForceBfs(long targetX, long targetY)
    {
        var start = (X: 1L, Y: 1L);
        var visited = new Set<(long X, long Y)>();
        var queue = new RepoQueue();
        queue.Enqueue(start);
        visited.TryAdd(start);

        while (queue.TryDequeue(out var point))
        {
            if (point.X == targetX && point.Y == targetY)
            {
                return true;
            }

            foreach (var next in Successors(point.X, point.Y))
            {
                if (next.X <= targetX && next.Y <= targetY && visited.TryAdd(next))
                {
                    queue.Enqueue(next);
                }
            }
        }

        return false;
    }

    private static IEnumerable<(long X, long Y)> Successors(long x, long y)
    {
        yield return (x + y, y);
        yield return (x, x + y);
        yield return (2 * x, y);
        yield return (x, 2 * y);
    }

    private static bool IsReachableByGcd(long targetX, long targetY)
    {
        var gcd = Gcd(targetX, targetY);
        return (gcd & (gcd - 1)) == 0;
    }

    private static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);
}
