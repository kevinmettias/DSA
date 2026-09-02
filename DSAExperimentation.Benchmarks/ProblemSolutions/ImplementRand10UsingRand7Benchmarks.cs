using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Implement Rand10() Using Rand7() (LC 470): NaiveModuloFold (a single Rand7() call
// folded via modulo) is the common first instinct - it's O(1) but statistically biased,
// since 7 doesn't divide 10 evenly, so it is included as the "fast but wrong" contrast,
// not a correctness baseline. RejectionSampling is the actual LeetCode-accepted answer:
// two Rand7() calls address a uniform 1..49 grid, retrying on the 9 cells beyond 40.
// Both call into the same seeded System.Random-backed Rand7(), this suite's established
// randomness-problem convention (see ShuffleAnArrayBenchmarks/RandomPickIndexBenchmarks).
[MemoryDiagnoser]
public class ImplementRand10UsingRand7Benchmarks
{
    private const int Rand7UpperBoundExclusive = 8;
    private const int Rand7RangeSize = 7;
    private const int RejectionThreshold = 40;
    private const int Rand10Range = 10;

    [Params(1_000, 100_000)]
    public int Calls;

    private Random _random = null!;

    [GlobalSetup]
    public void Setup() => _random = new Random(1);

    private int Rand7() => _random.Next(1, Rand7UpperBoundExclusive);

    [Benchmark(Baseline = true)]
    public int NaiveModuloFold()
    {
        var last = 0;
        for (var i = 0; i < Calls; i++)
        {
            last = 1 + (Rand7() - 1) % Rand10Range;
        }

        return last;
    }

    [Benchmark]
    public int RejectionSampling()
    {
        var last = 0;
        for (var i = 0; i < Calls; i++)
        {
            int index;
            do
            {
                var row = Rand7();
                var col = Rand7();
                index = (row - 1) * Rand7RangeSize + col;
            } while (index > RejectionThreshold);

            last = 1 + (index - 1) % Rand10Range;
        }

        return last;
    }
}
