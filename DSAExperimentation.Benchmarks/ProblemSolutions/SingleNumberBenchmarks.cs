using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SingleNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SingleNumberSolution's, the same methods
// SingleNumberSolutionTests proves correct. Values are paired up and shuffled so every
// element but one cancels; the singleton's position is randomized by the shuffle
// rather than fixed at an end. The XOR fold cancels in place while the set arm
// adds and removes every pair through a HashSet. The paired values are distinct and
// inside LC 136's [-3 * 10^4, 3 * 10^4], so each appears exactly twice, as the
// problem promises.
public class SingleNumberBenchmarks
{
    private const int ElementsPerPair = 2;
    private const int MaxPairedValue = 30_000;
    private const int Seed = 136;
    private const int SingletonValue = -1;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var pairCount = Length / ElementsPerPair;
        var pairs = SeededDraws.DistinctValues(pairCount, 1, MaxPairedValue + 1, random);

        var values = new List<int>(pairs.Length * ElementsPerPair + 1);
        values.AddRange(pairs);
        values.AddRange(pairs);
        values.Add(SingletonValue);

        _nums = [.. values.OrderBy(_ => random.Next())];
    }

    [Benchmark(Baseline = true)]
    public int XorFold() => SingleNumberSolution.FindUniqueByXorFold(_nums);

    [Benchmark]
    public int SetToggling() => SingleNumberSolution.FindUniqueBySetToggling(_nums);
}
