using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design an ATM Machine (LC 2241): a raw five-slot array greedy dispatch vs. this
// repo's own HashMap<int,long> keyed by denomination (DesignParkingSystemTests/
// DesignAnATMMachineTests precedent). Both are O(1) per call regardless of
// denomination-key space size, but unlike DesignParkingSystemBenchmarks' three-key
// case, the HashMap side is expected to lose here - hashing plus a bucket walk per
// lookup has a real constant-factor cost that plain index arithmetic into a 5-slot
// array never pays, the same "wrong tool, and that's the point" framing
// ShortestPathAlgorithmBenchmarks uses for FloydWarshall on a single-source query.
// Counts are seeded far above anything Calls could withdraw, so every withdrawal
// succeeds and both variants do identical work per call.
[MemoryDiagnoser]
public class DesignAnATMMachineBenchmarks
{
    private static readonly int[] DenominationsDescending = [500, 200, 100, 50, 20];
    private const long InitialCountPerDenomination = 1_000_000_000L;
    private const int AmountGranularity = 100;
    private const int MaxAmountMultiplier = 1_000;

    [Params(1_000, 50_000)]
    public int Calls;

    private long[] _amounts = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);

        // Multiples of 100 are always representable by this denomination set's
        // greedy walk given ample supply (500s/200s/100s alone cover any remainder
        // after subtracting as many 500s as fit), so every call below succeeds
        // identically for both variants instead of diverging on a failed withdrawal.
        _amounts = Enumerable.Range(0, Calls).Select(_ => (long)random.Next(1, MaxAmountMultiplier) * AmountGranularity).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long FiveFieldDispatch()
    {
        var counts = new long[DenominationsDescending.Length];

        for (var i = 0; i < counts.Length; i++)
        {
            counts[i] = InitialCountPerDenomination;
        }

        var totalWithdrawn = 0L;

        foreach (var amount in _amounts)
        {
            if (TryWithdrawArray(counts, amount))
            {
                totalWithdrawn += amount;
            }
        }

        return totalWithdrawn;
    }

    private static bool TryWithdrawArray(long[] counts, long amount)
    {
        var used = new long[counts.Length];
        var remaining = amount;

        for (var i = 0; i < DenominationsDescending.Length; i++)
        {
            var notes = Math.Min(counts[i], remaining / DenominationsDescending[i]);
            used[i] = notes;
            remaining -= notes * DenominationsDescending[i];
        }

        if (remaining != 0)
        {
            return false;
        }

        for (var i = 0; i < counts.Length; i++)
        {
            counts[i] -= used[i];
        }

        return true;
    }

    [Benchmark]
    public long HashMapDispatch()
    {
        var counts = new HashMap<int, long>();

        foreach (var denomination in DenominationsDescending)
        {
            counts.Set(denomination, InitialCountPerDenomination);
        }

        var totalWithdrawn = 0L;

        foreach (var amount in _amounts)
        {
            if (TryWithdrawHashMap(counts, amount))
            {
                totalWithdrawn += amount;
            }
        }

        return totalWithdrawn;
    }

    private static bool TryWithdrawHashMap(HashMap<int, long> counts, long amount)
    {
        var used = new long[DenominationsDescending.Length];
        var remaining = amount;

        for (var i = 0; i < DenominationsDescending.Length; i++)
        {
            var (notes, updatedRemaining) = TakeNotes(counts, DenominationsDescending[i], remaining);
            used[i] = notes;
            remaining = updatedRemaining;
        }

        if (remaining != 0)
        {
            return false;
        }

        for (var i = 0; i < DenominationsDescending.Length; i++)
        {
            counts.TryGetValue(DenominationsDescending[i], out var available);
            counts.Set(DenominationsDescending[i], available - used[i]);
        }

        return true;
    }

    private static (long Notes, long Remaining) TakeNotes(HashMap<int, long> counts, int denomination, long remaining)
    {
        counts.TryGetValue(denomination, out var available);
        var notes = Math.Min(available, remaining / denomination);
        return (notes, remaining - (notes * denomination));
    }
}
