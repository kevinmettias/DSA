using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ImplementRand10UsingRand7;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ImplementRand10UsingRand7Solution's, called Calls
// times against the same seeded System.Random-backed Rand7() (this suite's
// established randomness-problem convention - see ShuffleAnArrayBenchmarks,
// RandomPickIndexBenchmarks). NaiveModuloFold is the "fast but wrong" contrast, not
// a correctness baseline; RejectionSampling is the actual LeetCode-accepted answer.
[MemoryDiagnoser]
public class ImplementRand10UsingRand7Benchmarks
{
    private IRand7 _rand7 = null!;

    [Params(1_000, 100_000)]
    public int Calls { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _rand7 = new SeededRandomRand7(random);
    }

    [Benchmark(Baseline = true)]
    public int NaiveModuloFold()
    {
        var last = 0;

        for (var i = 0; i < Calls; i++)
        {
            last = ImplementRand10UsingRand7Solution.Rand10ByNaiveModuloFold(_rand7);
        }

        return last;
    }

    [Benchmark]
    public int RejectionSampling()
    {
        var last = 0;

        for (var i = 0; i < Calls; i++)
        {
            last = ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling(_rand7);
        }

        return last;
    }

    // The harness's own Rand7: a seeded System.Random drawn from per call. The seed is
    // the only thing this class holds, and it is what stands in for LeetCode's black
    // box - the same stand-in the tests use.
    private sealed class SeededRandomRand7(Random random) : IRand7
    {
        public int Draw() => random.Next(1, 8);
    }
}
