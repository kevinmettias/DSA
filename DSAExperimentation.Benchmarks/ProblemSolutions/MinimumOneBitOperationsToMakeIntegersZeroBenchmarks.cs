using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum One Bit Operations to Make Integers Zero (LC 1611): BFS over the implicit
// Gray-code path graph (this repo's own Queue<int> frontier + HashMap<int,int>
// distance map) vs. the O(log n) closed-form inverse-Gray-code loop. Both are
// correct - BFS walks the path one hop at a time, doing work proportional to the
// answer itself (which is within a small constant factor of n), while the closed
// form only ever touches n's own ~log2(n) bits once.
[MemoryDiagnoser]
public class MinimumOneBitOperationsToMakeIntegersZeroBenchmarks
{
    [Params(2_000, 50_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int BreadthFirstSearch() => MinimumOneBitOperationsBfs(N);

    [Benchmark]
    public int InverseGrayCodeFormula() => MinimumOneBitOperationsFormula(N);

    private static int MinimumOneBitOperationsBfs(int n)
    {
        if (n == 0)
        {
            return 0;
        }

        var (distances, frontier) = InitializeBfsState();
        return RunBfs(frontier, distances, n);
    }

    private static (HashMap<int, int> Distances, RepoQueue Frontier) InitializeBfsState()
    {
        var distances = new HashMap<int, int>();
        var frontier = new RepoQueue();
        distances.Set(0, 0);
        frontier.Enqueue(0);
        return (distances, frontier);
    }

    private static int RunBfs(RepoQueue frontier, HashMap<int, int> distances, int n)
    {
        while (frontier.TryDequeue(out var state))
        {
            distances.TryGetValue(state, out var distance);

            if (state == n)
            {
                return distance;
            }

            foreach (var neighbor in Neighbors(state))
            {
                if (!distances.HasKey(neighbor))
                {
                    distances.Set(neighbor, distance + 1);
                    frontier.Enqueue(neighbor);
                }
            }
        }

        return -1;
    }

    private static IEnumerable<int> Neighbors(int state)
    {
        yield return state ^ 1;

        if (state != 0)
        {
            yield return state ^ (1 << (LowestSetBitIndex(state) + 1));
        }
    }

    private static int LowestSetBitIndex(int value)
    {
        var index = 0;

        while ((value & 1) == 0)
        {
            value >>= 1;
            index++;
        }

        return index;
    }

    private static int MinimumOneBitOperationsFormula(int n)
    {
        var result = 0;

        while (n > 0)
        {
            result ^= n;
            n >>= 1;
        }

        return result;
    }
}
