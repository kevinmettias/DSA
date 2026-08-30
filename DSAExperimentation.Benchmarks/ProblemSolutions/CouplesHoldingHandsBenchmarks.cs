using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Couples Holding Hands (LC 765): the textbook greedy swap simulation (walk the row
// two seats at a time, and whenever a seat's occupant isn't already sitting beside
// their partner, look the partner up by position and swap them into place - the
// approach every LeetCode editorial leads with) vs. this repo's own DisjointSet
// unioning each seat pair's two occupants by couple id and counting distinct roots
// with this repo's own Set<int> (NumberOfProvincesBenchmarks' exact shape, applied
// to a permutation's cycle structure instead of an adjacency matrix). Both are O(n);
// the difference is swapping/position-array bookkeeping vs. near-constant-time
// Union-Find merging. The row is a random permutation of every seat so most couples
// start scattered instead of already paired.
[MemoryDiagnoser]
public class CouplesHoldingHandsBenchmarks
{
    [Params(200, 5_000)]
    public int CoupleCount;

    private int[] _row = null!;

    [GlobalSetup]
    public void Setup()
    {
        var seatCount = CoupleCount * 2;
        var row = new int[seatCount];

        for (var i = 0; i < seatCount; i++)
        {
            row[i] = i;
        }

        var random = new Random(765);

        for (var i = seatCount - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (row[i], row[j]) = (row[j], row[i]);
        }

        _row = row;
    }

    [Benchmark(Baseline = true)]
    public int GreedySwapSimulation()
    {
        var row = (int[])_row.Clone();
        var n = row.Length;
        var position = new int[n];

        for (var i = 0; i < n; i++)
        {
            position[row[i]] = i;
        }

        var swaps = 0;

        for (var seat = 0; seat < n; seat += 2)
        {
            var first = row[seat];
            var partner = first % 2 == 0 ? first + 1 : first - 1;

            if (row[seat + 1] == partner)
            {
                continue;
            }

            var partnerSeat = position[partner];
            var displaced = row[seat + 1];

            row[seat + 1] = partner;
            row[partnerSeat] = displaced;
            position[partner] = seat + 1;
            position[displaced] = partnerSeat;
            swaps++;
        }

        return swaps;
    }

    [Benchmark]
    public int DisjointSetComponentCounting()
    {
        var coupleCount = _row.Length / 2;
        var couples = new DisjointSet(coupleCount);

        for (var seat = 0; seat < _row.Length; seat += 2)
        {
            couples.Union(_row[seat] / 2, _row[seat + 1] / 2);
        }

        var roots = new Set<int>();

        for (var couple = 0; couple < coupleCount; couple++)
        {
            roots.TryAdd(couples.Find(couple));
        }

        return coupleCount - roots.Count;
    }
}
