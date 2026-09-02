using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheNumberOfPossibleWaysForAnEvent;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheNumberOfPossibleWaysForAnEventSolution's.
// Stages and score range stay fixed while performer count grows, so the brute
// force's x^n enumeration is what the memoized DP has to beat.
[MemoryDiagnoser]
public class FindTheNumberOfPossibleWaysForAnEventBenchmarks
{
    private const int Stages = 3;
    private const int MaxScore = 5;

    [Params(6, 10)]
    public int Performers;

    [Benchmark(Baseline = true)]
    public int BruteForceEnumeration() =>
        FindTheNumberOfPossibleWaysForAnEventSolution.NumberOfWaysByBruteForceEnumeration(Performers, Stages, MaxScore);

    [Benchmark]
    public int StagePartitionMemo() =>
        FindTheNumberOfPossibleWaysForAnEventSolution.NumberOfWaysByStagePartitionMemo(Performers, Stages, MaxScore);
}
