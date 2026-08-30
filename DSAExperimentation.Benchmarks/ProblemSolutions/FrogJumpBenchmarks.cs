using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Frog Jump (LC 403): the textbook un-memoized recursive DFS (retrying every
// {-1,0,+1} jump-size delta with a binary search for the landing stone) vs.
// this repo's own HashMap<TKey,TValue>, nested as HashMap<int, HashMap<int,
// bool>> to record every jump size already known to reach each stone so no
// state is ever re-explored. _stones is built as consecutive integers (every
// jump delta from every reachable stone lands on another real stone) with an
// unreachable final stone appended far away - the same "rig the input so
// both strategies are forced through their full worst case" trick
// TwoSumBenchmarks/JumpGameBenchmarks use, here forcing RecursiveBruteForce
// through its full exponential search instead of returning early on success.
[MemoryDiagnoser]
public class FrogJumpBenchmarks
{
    [Params(10, 16)]
    public int StoneCount;

    private int[] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        _stones = new int[StoneCount];

        for (var i = 0; i < StoneCount - 1; i++)
        {
            _stones[i] = i;
        }

        _stones[StoneCount - 1] = StoneCount - 2 + 1_000;
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveBruteForce() => TryJump(_stones, 0, 0);

    private static bool TryJump(int[] stones, int index, int lastJump)
    {
        if (index == stones.Length - 1)
        {
            return true;
        }

        for (var delta = -1; delta <= 1; delta++)
        {
            var jump = lastJump + delta;

            if (jump <= 0)
            {
                continue;
            }

            var nextIndex = Array.BinarySearch(stones, index + 1, stones.Length - index - 1, stones[index] + jump);

            if (nextIndex >= 0 && TryJump(stones, nextIndex, jump))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool HashMapDynamicProgramming()
    {
        var jumpsByStone = new HashMap<int, HashMap<int, bool>>();

        foreach (var stone in _stones)
        {
            jumpsByStone.Set(stone, new HashMap<int, bool>());
        }

        jumpsByStone.TryGetValue(_stones[0], out var startJumps);
        startJumps.Set(0, true);

        foreach (var stone in _stones)
        {
            jumpsByStone.TryGetValue(stone, out var jumps);

            foreach (var jump in jumps.Keys)
            {
                for (var delta = -1; delta <= 1; delta++)
                {
                    var nextJump = jump + delta;

                    if (nextJump <= 0)
                    {
                        continue;
                    }

                    if (jumpsByStone.TryGetValue(stone + nextJump, out var nextJumps))
                    {
                        nextJumps.Set(nextJump, true);
                    }
                }
            }
        }

        jumpsByStone.TryGetValue(_stones[^1], out var lastJumps);
        return lastJumps.Count > 0;
    }
}
