using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.ClimbingStairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClimbingStairsSolution's, timed the way LeetCode times a submission -
// once across every case it holds rather than at one step count. The cases are the LeetCode
// catalog's for LC 70, its published examples and LeetCodeDataset's; ClimbingStairsBenchmarks'
// step counts show how each arm grows. Each arm writes its answers into a buffer sized in
// [GlobalSetup].
public class ClimbingStairsSuiteBenchmarks
{
    private const string TitleSlug = "climbing-stairs";

    private int[] _stepCounts = [];
    private int[] _answers = [];

    [GlobalSetup]
    public void Setup()
    {
        _stepCounts = [.. LeetCodeCatalog.Cases(TitleSlug).Select(testCase => testCase.Input<int>(0))];
        _answers = new int[_stepCounts.Length];
    }

    [Benchmark(Baseline = true)]
    public int[] IterativeRollingTotals()
    {
        for (var index = 0; index < _stepCounts.Length; index++)
        {
            _answers[index] = ClimbingStairsSolution.CountWaysByIterativeRollingTotals(_stepCounts[index]);
        }

        return _answers;
    }

    [Benchmark]
    public int[] MemoizedRecurrence()
    {
        for (var index = 0; index < _stepCounts.Length; index++)
        {
            _answers[index] = ClimbingStairsSolution.CountWaysByMemoizedRecurrence(_stepCounts[index]);
        }

        return _answers;
    }
}
