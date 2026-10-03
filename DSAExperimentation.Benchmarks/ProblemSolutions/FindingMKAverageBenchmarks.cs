using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindingMKAverage;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindingMKAverageSolution's, the same factories
// FindingMKAverageSolutionTests proves correct. [GlobalSetup] builds one fixed element
// stream, so stream construction is charged to setup and only the replay - an
// addElement plus a calculateMKAverage per element, identical for both arms - is
// measured. Each arm returns every MK average the replay calculated, in order.
public class FindingMKAverageBenchmarks
{
    private const int WindowSize = 99;
    private const int TrimCount = 33;

    // LC problem number, reused as the deterministic stream seed.
    private const int RandomSeed = 1825;
    private const int MaxElementValue = 100_000;

    private int[] _stream = [];

    // Every MK average the replay calculates; sized in setup so the replay allocates nothing.
    private int[] _averages = [];

    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stream = SeededDraws.Values(Length, 1, MaxElementValue, random);
        _averages = new int[_stream.Length];
    }

    [Benchmark(Baseline = true)]
    public int[] SortingSlidingWindow()
    {
        var mkAverage = FindingMKAverageSolution.CreateBySortingSlidingWindow(WindowSize, TrimCount);
        return Replay(mkAverage);
    }

    [Benchmark]
    public int[] FenwickOrderStatistics()
    {
        var mkAverage = FindingMKAverageSolution.CreateByFenwickOrderStatistics(WindowSize, TrimCount);
        return Replay(mkAverage);
    }

    private int[] Replay(FindingMKAverageSolution.IMKAverage mkAverage)
    {
        for (var i = 0; i < _stream.Length; i++)
        {
            mkAverage.AddElement(_stream[i]);
            _averages[i] = mkAverage.CalculateMKAverage();
        }

        return _averages;
    }
}
