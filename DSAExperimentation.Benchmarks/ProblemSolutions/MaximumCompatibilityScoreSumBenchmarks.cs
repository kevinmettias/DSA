using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumCompatibilityScoreSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumCompatibilityScoreSumSolution's, the same methods
// MaximumCompatibilityScoreSumTests proves correct - the textbook unmemoized bitmask
// recursion over (student, usedMentorMask) against the identical recursion routed
// through this repo's own Memoizer. Each arm is handed the prepared
// CompatibilityScoreMatrix its hoisted overload takes, so building the score table is
// charged to [GlobalSetup] rather than to the search being measured.
[MemoryDiagnoser]
public class MaximumCompatibilityScoreSumBenchmarks
{
    private const int QuestionCount = 8;

    // LC problem number, reused as the deterministic answer-sheet seed.
    private const int RandomSeed = 1947;

    [Params(4, 7)]
    public int GroupSize;

    private CompatibilityScoreMatrix _scores = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (students, mentors) =
            CompatibilitySurveyWorkloads.BuildAnswerSheets(GroupSize, QuestionCount, seed: RandomSeed);

        _scores = CompatibilityScoreMatrix.Build(students, mentors);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        MaximumCompatibilityScoreSumSolution.MaxCompatibilitySumByBruteForceRecursion(_scores);

    [Benchmark]
    public int MemoizedBitmask() =>
        MaximumCompatibilityScoreSumSolution.MaxCompatibilitySumByMemoizedBitmask(_scores);
}
