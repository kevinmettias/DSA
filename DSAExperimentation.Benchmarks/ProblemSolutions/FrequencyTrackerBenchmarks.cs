using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FrequencyTracker;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FrequencyTrackerSolution's, the same factories
// FrequencyTrackerTests proves correct. [GlobalSetup] builds one fixed call script -
// Length adds drawn from a small value range so numbers genuinely repeat, a quarter
// of them deleted again, then a batch of hasFrequency queries - so script
// construction is charged to setup and only the replay is measured. SortedScanList is
// the "no hashing at all" baseline (a raw List<int>, IndexOf deletes, and a sort per
// query) against PairedHashMaps' two HashMap<TKey,TValue> instances, the same
// contrast DesignHashMapBenchmarks already establishes for LC 706.
[MemoryDiagnoser]
public class FrequencyTrackerBenchmarks
{
    private const int Seed = 2671; // LC problem number
    private const int MinQueryCount = 20;
    private const int QueryCountDivisor = 10;
    private const int DeleteCountDivisor = 4;

    [Params(200, 5_000)]
    public int Length;

    private int[] _numbersToAdd = null!;
    private int[] _numbersToDelete = null!;
    private int[] _frequenciesToQuery = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var valueRange = Math.Max(1, Length / DeleteCountDivisor);

        _numbersToAdd = Enumerable.Range(0, Length).Select(_ => random.Next(valueRange)).ToArray();
        _numbersToDelete = _numbersToAdd.Take(Length / DeleteCountDivisor).ToArray();

        var queryCount = Math.Max(MinQueryCount, Length / QueryCountDivisor);
        _frequenciesToQuery = Enumerable.Range(0, queryCount).Select(_ => random.Next(1, valueRange + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SortedScanList() => Replay(FrequencyTrackerSolution.CreateBySortedScanList());

    [Benchmark]
    public int PairedHashMaps() => Replay(FrequencyTrackerSolution.CreateByPairedHashMaps());

    private int Replay(FrequencyTrackerSolution.IFrequencyTracker tracker)
    {
        foreach (var number in _numbersToAdd)
        {
            tracker.Add(number);
        }

        foreach (var number in _numbersToDelete)
        {
            tracker.DeleteOne(number);
        }

        var trueCount = 0;

        foreach (var frequency in _frequenciesToQuery)
        {
            if (tracker.HasFrequency(frequency))
            {
                trueCount++;
            }
        }

        return trueCount;
    }
}
