using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PermutationInString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationInStringSolution's, the same methods
// PermutationInStringTests proves correct. s1 is fixed and deliberately absent
// from s2 so both strategies are forced through their full worst-case scan.
[MemoryDiagnoser]
public class PermutationInStringBenchmarks
{
    private const string Pattern = "aeiou";
    private const int RandomSeed = 567; private string _s2 = "";

    // LC problem number

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _s2 = PermutationInStringWorkloads.BuildHaystack(Length, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public bool PerWindowFrequencyRebuild() =>
        PermutationInStringSolution.CheckInclusionByPerWindowRebuild(
            new PermutationPattern(Pattern), new SearchedText(_s2));

    [Benchmark]
    public bool SlidingWindowFrequencyMap() =>
        PermutationInStringSolution.CheckInclusionBySlidingWindow(
            new PermutationPattern(Pattern), new SearchedText(_s2));
}
