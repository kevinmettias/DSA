using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BoatsToSavePeople;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BoatsToSavePeopleSolution's, the same methods
// BoatsToSavePeopleTests proves correct. Weights are drawn from the whole range
// below the limit, so most people have a partner who fits and brute force pays a
// full rescan per boat instead of sending everyone alone.
[MemoryDiagnoser]
public class BoatsToSavePeopleBenchmarks
{
    private const int Limit = 300;
    private const int RandomSeed = 881; private int[] _people = [];

    // LC problem number

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _people = Enumerable.Range(0, Length).Select(_ => random.Next(1, Limit)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRepeatedScan() =>
        BoatsToSavePeopleSolution.NumberOfRescueBoatsByRepeatedScan(_people, Limit);

    [Benchmark]
    public int SortThenTwoPointer() =>
        BoatsToSavePeopleSolution.NumberOfRescueBoatsBySortThenTwoPointer(_people, Limit);
}
