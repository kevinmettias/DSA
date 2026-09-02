using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Ways to Rearrange Sticks With K Sticks Visible (LC 1866): the textbook
// O(n!) brute force - enumerate every permutation of [1..StickCount] and count how
// many have exactly VisibleCount left-to-right visible sticks - vs. this repo's own
// Memoizer<TState,TResult> computing the identical count via the unsigned-Stirling-
// number recurrence in O(StickCount*VisibleCount) states. StickCount is kept modest
// for the same reason StoneGameVIIBenchmarks keeps PileCount modest for its own
// exponential baseline - the brute force's factorial blowup is real.
[MemoryDiagnoser]
public class NumberOfWaysToRearrangeSticksWithKSticksVisibleBenchmarks
{
    private const int Mod = 1_000_000_007;
    private const int VisibleCount = 3;

    [Params(8, 9)]
    public int StickCount;

    [Benchmark(Baseline = true)]
    public long BruteForcePermutations()
    {
        var sticks = new int[StickCount];
        for (var i = 0; i < StickCount; i++)
        {
            sticks[i] = i + 1;
        }

        var count = 0L;
        Permute(sticks, 0, ref count);
        return count;
    }

    private static void Permute(int[] sticks, int index, ref long matchingCount)
    {
        if (index == sticks.Length)
        {
            if (CountVisible(sticks) == VisibleCount)
            {
                matchingCount++;
            }

            return;
        }

        for (var i = index; i < sticks.Length; i++)
        {
            (sticks[index], sticks[i]) = (sticks[i], sticks[index]);
            Permute(sticks, index + 1, ref matchingCount);
            (sticks[index], sticks[i]) = (sticks[i], sticks[index]);
        }
    }

    private static int CountVisible(int[] sticks)
    {
        var visible = 0;
        var tallestSoFar = 0;

        foreach (var stick in sticks)
        {
            if (stick > tallestSoFar)
            {
                visible++;
                tallestSoFar = stick;
            }
        }

        return visible;
    }

    [Benchmark]
    public long MemoizedStirlingRecurrence()
        => Memoizer.Memoize<(int N, int K), long>((StickCount, VisibleCount), Ways);

    private static long Ways((int N, int K) state, Func<(int N, int K), long> ways)
    {
        var (n, k) = state;

        if (n == 0)
        {
            return k == 0 ? 1 : 0;
        }

        if (k == 0)
        {
            return 0;
        }

        var placeAsNewVisible = ways((n - 1, k - 1));
        var hideAfterExistingStick = (n - 1) * ways((n - 1, k)) % Mod;
        return (placeAsNewVisible + hideAfterExistingStick) % Mod;
    }
}
