using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindingMKAverage;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindingMKAverageSolution's, the same factories
// FindingMKAverageTests proves correct. [GlobalSetup] builds one fixed element
// stream, so stream construction is charged to setup and only the replay - an
// addElement plus a calculateMKAverage per element, identical for both arms - is
// measured.
[MemoryDiagnoser]
public class FindingMKAverageBenchmarks
{
    private const int WindowSize = 99;
    private const int K = 33;

    // LC problem number, reused as the deterministic stream seed.
    private const int RandomSeed = 1825;
    private const int MaxElementValue = 100_000;

    private int[] _stream = [];

    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stream = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long SortingSlidingWindow()
    {
        var mkAverage = FindingMKAverageSolution.CreateBySortingSlidingWindow(WindowSize, K);
        return Replay(mkAverage);
    }

    [Benchmark]
    public long FenwickOrderStatistics()
    {
        var mkAverage = FindingMKAverageSolution.CreateByFenwickOrderStatistics(WindowSize, K);
        return Replay(mkAverage);
    }

    private long Replay(FindingMKAverageSolution.IMKAverage mkAverage)
    {
        var checksum = 0L;

        foreach (var value in _stream)
        {
            mkAverage.AddElement(value);
            var result = mkAverage.CalculateMKAverage();
            checksum += result < 0 ? 0 : result;
        }

        return checksum;
    }
}
