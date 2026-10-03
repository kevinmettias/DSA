using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FrequencyTracker;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FrequencyTrackerSolution's, the same factories
// FrequencyTrackerSolutionTests proves correct. [GlobalSetup] builds one fixed call script -
// Length adds drawn from a small value range so numbers genuinely repeat, a quarter
// of them deleted again, then a batch of hasFrequency queries - so script
// construction is charged to setup and only the replay is measured. SortedScanList is
// the "no hashing at all" baseline (a raw List<int>, IndexOf deletes, and a sort per
// query) against PairedHashMaps' two HashMap<TKey,TValue> instances, the same
// contrast DesignHashMapBenchmarks already establishes for LC 706. Each arm returns
// every hasFrequency answer, in query order. Numbers are drawn from 1 up, LC 2671's
// smallest.
public class FrequencyTrackerBenchmarks
{
    private const int Seed = 2671; // LC problem number
    private const int MinQueryCount = 20;
    private const int QueryCountDivisor = 10;
    private const int DeleteCountDivisor = 4;

    private int[] _numbersToAdd = [];

    private int[] _numbersToDelete = [];
    private int[] _frequenciesToQuery = [];

    // Every hasFrequency answer; sized in setup so the replay allocates nothing.
    private bool[] _answers = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var valueRange = Math.Max(1, Length / DeleteCountDivisor);

        _numbersToAdd = SeededDraws.Values(Length, 1, valueRange + 1, random);
        _numbersToDelete = _numbersToAdd.Take(Length / DeleteCountDivisor).ToArray();

        var queryCount = Math.Max(MinQueryCount, Length / QueryCountDivisor);
        _frequenciesToQuery = SeededDraws.Values(queryCount, 1, valueRange + 1, random);
        _answers = new bool[_frequenciesToQuery.Length];
    }

    [Benchmark(Baseline = true)]
    public bool[] SortedScanList() => Replay(FrequencyTrackerSolution.CreateBySortedScanList());

    [Benchmark]
    public bool[] PairedHashMaps() => Replay(FrequencyTrackerSolution.CreateByPairedHashMaps());

    private bool[] Replay(FrequencyTrackerSolution.IFrequencyTracker tracker)
    {
        foreach (var number in _numbersToAdd)
        {
            tracker.Add(number);
        }

        foreach (var number in _numbersToDelete)
        {
            tracker.DeleteOne(number);
        }

        for (var i = 0; i < _frequenciesToQuery.Length; i++)
        {
            _answers[i] = tracker.HasFrequency(_frequenciesToQuery[i]);
        }

        return _answers;
    }
}
