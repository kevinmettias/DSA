using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Random Flip Matrix (LC 519): a naive materialized baseline (a List<int> of every
// not-yet-flipped cell index, each flip picking a random List position and
// RemoveAt-ing it - an O(n) shift of every subsequent element) vs. this repo's
// HashMap<int,int> "swap the picked slot with the last slot" composition
// (InsertDeleteGetRandomO1Benchmarks precedent) - O(1) amortized per flip, with no
// materialized array at all. Both drain the same [0, Cells) universe with the same
// seeded Random and return a checksum of every picked value, so a mismatched
// checksum would mean the two approaches disagree, not just run at different speeds.
[MemoryDiagnoser]
public class RandomFlipMatrixBenchmarks
{
    [Params(200, 5_000)]
    public int Cells;

    [Benchmark(Baseline = true)]
    public long ListBased()
    {
        var values = new List<int>(Cells);
        for (var i = 0; i < Cells; i++)
        {
            values.Add(i);
        }

        var random = new Random(1);
        long checksum = 0;

        while (values.Count > 0)
        {
            var pick = random.Next(values.Count);
            checksum += values[pick];
            values.RemoveAt(pick);
        }

        return checksum;
    }

    [Benchmark]
    public long HashMapSwapBased()
    {
        var swapped = new HashMap<int, int>();
        var random = new Random(1);
        var remaining = Cells;
        long checksum = 0;

        while (remaining > 0)
        {
            var pick = random.Next(remaining);
            var value = swapped.TryGetValue(pick, out var mapped) ? mapped : pick;
            checksum += value;

            remaining--;
            var lastValue = swapped.TryGetValue(remaining, out var lastMapped) ? lastMapped : remaining;
            swapped.Set(pick, lastValue);
        }

        return checksum;
    }
}
