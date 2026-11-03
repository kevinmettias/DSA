using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.LeetCode.Solutions.PermutationInString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationInStringSolution's, the same methods
// PermutationInStringSolutionTests proves correct. s1 is fixed and deliberately absent
// from s2 so both strategies are forced through their full worst-case scan.
public class PermutationInStringBenchmarks
{
    private const int RandomSeed = 567; private string _s2 = "";
    private static readonly string Pattern = EnglishVowels.LettersAsString();

    // LC problem number

    [Params(2_000, 10_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _s2 = PermutationInStringWorkloads.BuildHaystack(Length, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public bool HasPermutationByPerWindowRebuild() =>
        PermutationInStringSolution.HasPermutationByPerWindowRebuild(
            new PermutationPattern(Pattern), new SearchedText(_s2));

    [Benchmark]
    public bool HasPermutationBySlidingWindow() =>
        PermutationInStringSolution.HasPermutationBySlidingWindow(
            new PermutationPattern(Pattern), new SearchedText(_s2));
}
