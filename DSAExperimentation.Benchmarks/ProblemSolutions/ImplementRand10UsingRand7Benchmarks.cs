using DSAExperimentation.LeetCode.ImplementRand10UsingRand7;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ImplementRand10UsingRand7Solution's, called Calls
// times against the same seeded System.Random-backed Rand7() (this suite's
// established randomness-problem convention - see ShuffleAnArrayBenchmarks,
// RandomPickIndexBenchmarks). RejectionSampling is the textbook answer, retrying
// from scratch on a rejected pair of draws; RecycledRejectionSampling is LeetCode's
// follow-up, carrying a rejected draw's leftover randomness into the next round for
// about 2.19 Rand7() calls per Rand10() instead of 2.45. Calls runs to LeetCode's
// 10^5. Each arm returns every value it drew, in call order.
// Each call seeds a fresh Rand7 rather than sharing one built in [GlobalSetup]: both
// strategies advance the generator they are handed, so a shared one would hand every
// call after the first different draws from the ones the tests check. Seeding it is
// timed on purpose, and every arm pays the same cost.
public class ImplementRand10UsingRand7Benchmarks
{
    private int[] _draws = [];

    [Params(1_000, 100_000)]
    public int Calls { get; set; }

    [GlobalSetup]
    public void Setup() => _draws = new int[Calls];

    [Benchmark(Baseline = true)]
    public int[] RejectionSampling()
    {
        var rand7 = new SeededRandomRand7(new Random(1));

        for (var i = 0; i < Calls; i++)
        {
            _draws[i] = ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling(rand7);
        }

        return _draws;
    }

    [Benchmark]
    public int[] RecycledRejectionSampling()
    {
        var rand7 = new SeededRandomRand7(new Random(1));

        for (var i = 0; i < Calls; i++)
        {
            _draws[i] = ImplementRand10UsingRand7Solution.Rand10ByRecycledRejectionSampling(rand7);
        }

        return _draws;
    }

    // The harness's own Rand7: a seeded System.Random drawn from per call. The seed is
    // the only thing this class holds, and it is what stands in for LeetCode's black
    // box - the same stand-in the tests use.
    private sealed class SeededRandomRand7(Random random) : IRand7
    {
        public int Draw() => random.Next(1, 8);
    }
}
