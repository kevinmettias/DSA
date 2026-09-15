using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.Dota2Senate;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are Dota2SenateSolution's, the same methods
// Dota2SenateTests proves correct. Every 'R' seat first, every 'D' seat after, so
// early rounds of the circular-rescan baseline each skip past nearly the whole
// opposing block before finding their target - the O(n^2) worst case that strategy
// is deliberately shaped to hit.
[MemoryDiagnoser]
public class Dota2SenateBenchmarks
{
    // Splits SenatorCount in half so the input is exactly one full block of each party.
    private const int PartySplitDivisor = 2;

    private string _senate = "";

    [Params(500, 20_000)]
    public int SenatorCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var senate = new char[SenatorCount];
        var half = SenatorCount / PartySplitDivisor;

        for (var i = 0; i < SenatorCount; i++)
        {
            senate[i] = i < half ? 'R' : 'D';
        }

        _senate = new string(senate);
    }

    [Benchmark(Baseline = true)]
    public string CircularRescanSimulation() => Dota2SenateSolution.PredictPartyVictoryByCircularRescan(_senate);

    [Benchmark]
    public string TwoQueueSimulation() => Dota2SenateSolution.PredictPartyVictoryByTwoQueueSimulation(_senate);
}
