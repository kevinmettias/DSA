using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SingleNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is SingleNumberSolution's XOR fold, the same method
// SingleNumberTests proves correct. Values are paired up and shuffled so every
// element but one cancels; the singleton's position is randomized by the shuffle
// rather than fixed at an end.
[MemoryDiagnoser]
public class SingleNumberBenchmarks
{
    private const int ElementsPerPair = 2;
    private const int ValueUpperBound = 1_000_000;
    private const int Seed = 136;
    private const int SingletonValue = -1;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var pairCount = Length / ElementsPerPair;
        var pairs = Enumerable.Range(0, pairCount).Select(_ => random.Next(1, ValueUpperBound)).ToArray();

        var values = new List<int>(pairs.Length * ElementsPerPair + 1);
        values.AddRange(pairs);
        values.AddRange(pairs);
        values.Add(SingletonValue);

        _nums = [.. values.OrderBy(_ => random.Next())];
    }

    [Benchmark]
    public int XorFold() => SingleNumberSolution.FindUniqueByXorFold(_nums);
}
