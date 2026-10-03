using DSAExperimentation.LeetCode.DesignAnATMMachine;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAnATMMachineSolution's, the same classes
// DesignAnATMMachineSolutionTests proves correct - a raw five-slot array greedy dispatch
// against this repo's own HashMap<int, long> keyed by denomination
// (DesignParkingSystemBenchmarks precedent). Both are O(1) per call regardless of
// denomination-key space size, but unlike that three-key case the HashMap side is
// expected to lose here: hashing plus a bucket walk per lookup has a real
// constant-factor cost that plain index arithmetic into a five-slot array never
// pays, the same "wrong tool, and that's the point" framing
// ShortestPathAlgorithmBenchmarks uses for FloydWarshall on a single-source query.
// [GlobalSetup] materializes the amount script so random generation is charged to
// setup rather than to the replay, and counts are seeded far above anything Calls
// could withdraw, so every withdrawal succeeds and both arms do identical work per
// call.
public class DesignAnATMMachineBenchmarks
{
    private const long InitialCountPerDenomination = 1_000_000_000L;
    private const int AmountGranularity = 100;
    private const int MaxAmountMultiplier = 1_000;
    private const int RandomSeed = 1;

    private long[] _amounts = [];

    private long[] _openingDeposit = [];

    // The notes every Withdraw hands back, in call order; sized in setup so the replay
    // allocates nothing beyond what the strategy itself returns.
    private long[][] _withdrawn = [];
    [Params(1_000, 50_000)]
    public int Calls { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Multiples of 100 are always representable by this denomination set's
        // greedy walk given ample supply (500s/200s/100s alone cover any remainder
        // after subtracting as many 500s as fit), so every call below succeeds
        // identically for both variants instead of diverging on a failed withdrawal.
        _amounts = Enumerable.Range(0, Calls).Select(_ => (long)random.Next(1, MaxAmountMultiplier) * AmountGranularity).ToArray();
        _openingDeposit = Enumerable.Repeat(InitialCountPerDenomination, DesignAnATMMachineSolution.DenominationCount).ToArray();
        _withdrawn = new long[Calls][];
    }

    [Benchmark(Baseline = true)]
    public long[][] FiveSlotArrayDispatch() => Replay(new DesignAnATMMachineSolution.AtmByFiveSlotArray());

    [Benchmark]
    public long[][] HashMapDispatch() => Replay(new DesignAnATMMachineSolution.AtmByHashMap());

    private long[][] Replay(DesignAnATMMachineSolution.IAtm atm)
    {
        atm.Deposit(_openingDeposit);

        for (var i = 0; i < _amounts.Length; i++)
        {
            _withdrawn[i] = atm.Withdraw(_amounts[i]);
        }

        return _withdrawn;
    }
}
