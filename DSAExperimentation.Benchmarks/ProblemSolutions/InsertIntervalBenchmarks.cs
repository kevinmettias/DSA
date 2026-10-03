using DSAExperimentation.LeetCode.InsertInterval;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InsertIntervalSolution's, the same methods
// InsertIntervalSolutionTests proves correct. Each arm returns the merged intervals.
public class InsertIntervalBenchmarks
{
    private const int IntervalSpacing = 3;
    private const int NewIntervalEndMultiplier = 2;

    private (int Start, int End)[] _intervals = [];

    private (int Start, int End) _newInterval;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _intervals = Enumerable.Range(0, Length).Select(i => (i * IntervalSpacing, (i * IntervalSpacing) + 1)).ToArray();
        _newInterval = (Length, Length * NewIntervalEndMultiplier);
    }

    [Benchmark(Baseline = true)]
    public List<(int Start, int End)> ListInsertAndMerge() =>
        InsertIntervalSolution.InsertByListSortAndMerge(_intervals, _newInterval);

    [Benchmark]
    public List<(int Start, int End)> IntervalSetAdd() =>
        InsertIntervalSolution.InsertByIntervalSet(_intervals, _newInterval);
}
