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
    [Params(1_000, 100_000)]
    public int Calls;

    private Func<int> _rand7 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _rand7 = () => random.Next(1, 8);
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
}
