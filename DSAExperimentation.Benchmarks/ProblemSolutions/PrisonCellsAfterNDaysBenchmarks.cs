using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Prison Cells After N Days (LC 957): the naive day-by-day simulation (baseline)
// walks all N days directly - fine for small N, but scales linearly even though
// the 8-cell state space has only 256 possible encodings and the sequence of
// states must cycle almost immediately in practice. This repo's own
// HashMap<int,int> maps each encoded state to the day index it was first seen,
// so the cycle-detection strategy jumps straight to N modulo the cycle length
// instead of looping N times - near-constant time regardless of how large N gets.
[MemoryDiagnoser]
public class PrisonCellsAfterNDaysBenchmarks
{
    [Params(10_000, 1_000_000)]
    public int Days;

    private int[] _cells = null!;

    [GlobalSetup]
    public void Setup() => _cells = [1, 0, 0, 1, 0, 0, 1, 0];

    [Benchmark(Baseline = true)]
    public int[] BruteForceSimulation()
    {
        var state = Encode(_cells);

        for (var day = 0; day < Days; day++)
        {
            state = NextState(state);
        }

        return Decode(state);
    }

    [Benchmark]
    public int[] CycleDetectionViaHashMap()
    {
        var seenAtDay = new HashMap<int, int>();
        var state = Encode(_cells);
        var day = 0;

        while (day < Days)
        {
            if (seenAtDay.TryGetValue(state, out var firstSeenDay))
            {
                var cycleLength = day - firstSeenDay;
                var remaining = (Days - day) % cycleLength;

                for (var i = 0; i < remaining; i++)
                {
                    state = NextState(state);
                }

                return Decode(state);
            }

            seenAtDay.Set(state, day);
            state = NextState(state);
            day++;
        }

        return Decode(state);
    }

    private static int NextState(int state)
    {
        var next = 0;

        for (var i = 1; i < 7; i++)
        {
            var left = (state >> (i - 1)) & 1;
            var right = (state >> (i + 1)) & 1;

            if (left == right)
            {
                next |= 1 << i;
            }
        }

        return next;
    }

    private static int Encode(int[] cells)
    {
        var state = 0;

        for (var i = 0; i < cells.Length; i++)
        {
            if (cells[i] == 1)
            {
                state |= 1 << i;
            }
        }

        return state;
    }

    private static int[] Decode(int state)
    {
        var cells = new int[8];

        for (var i = 0; i < 8; i++)
        {
            cells[i] = (state >> i) & 1;
        }

        return cells;
    }
}
