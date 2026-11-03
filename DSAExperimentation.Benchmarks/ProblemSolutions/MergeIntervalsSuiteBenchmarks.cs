using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.MergeIntervals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeIntervalsSolution's, timed the way LeetCode times a submission -
// once across every case it holds rather than at one length. The cases are the LeetCode catalog's
// for LC 56, its published examples and LeetCodeDataset's, translated from LeetCode's two-element
// rows to (start, end) pairs in [GlobalSetup]; MergeIntervalsBenchmarks' lengths show how each arm
// grows. Each arm writes its answers into a buffer sized there.
public class MergeIntervalsSuiteBenchmarks
{
    private const string TitleSlug = "merge-intervals";

    private (int Start, int End)[][] _cases = [];
    private List<(int Start, int End)>[] _answers = [];

    [GlobalSetup]
    public void Setup()
    {
        _cases = [.. LeetCodeCatalog.Cases(TitleSlug).Select(testCase => PairsOf(testCase.Input<int[][]>(0)))];
        _answers = new List<(int Start, int End)>[_cases.Length];
    }

    private static (int Start, int End)[] PairsOf(int[][] rows) => [.. rows.Select(row => (row[0], row[1]))];

    [Benchmark(Baseline = true)]
    public List<(int Start, int End)>[] BatchSortAndMerge()
    {
        for (var index = 0; index < _cases.Length; index++)
        {
            _answers[index] = MergeIntervalsSolution.MergeByBatchSortAndMerge(_cases[index]);
        }

        return _answers;
    }

    [Benchmark]
    public List<(int Start, int End)>[] IncrementalIntervalSet()
    {
        for (var index = 0; index < _cases.Length; index++)
        {
            _answers[index] = MergeIntervalsSolution.MergeByIntervalSet(_cases[index]);
        }

        return _answers;
    }
}
