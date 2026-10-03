using DSAExperimentation.LeetCode.InsertDeleteGetRandomO1;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InsertDeleteGetRandomO1Solution's, the same classes
// InsertDeleteGetRandomO1SolutionTests proves correct. A Design problem's whole point is a
// sequence of mutating calls against one instance, so there is no separate "prepare
// input" step to hoist into [GlobalSetup] beyond the insert/removal order arrays
// themselves - [GlobalSetup] builds those (so shuffling isn't charged to the measured
// method) and each [Benchmark] arm constructs its own instance and replays the same
// insert-then-remove script, returning every Insert and Remove verdict, in call order,
// so the JIT can't eliminate the replay as dead code.
public class InsertDeleteGetRandomO1Benchmarks
{
    private int[] _insertOrder = [];

    private int[] _removalOrder = [];

    // Every Insert verdict, then every Remove verdict; sized in setup so the replay allocates nothing.
    private bool[] _verdicts = [];
    [Params(200, 20_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _insertOrder = Enumerable.Range(0, Count).ToArray();

        var random = new Random(1);
        _removalOrder = _insertOrder.OrderBy(_ => random.Next()).ToArray();
        _verdicts = new bool[_insertOrder.Length + _removalOrder.Length];
    }

    [Benchmark(Baseline = true)]
    public bool[] ListScan() => Replay(new InsertDeleteGetRandomO1Solution.RandomizedSetByListScan());

    [Benchmark]
    public bool[] HashMapSwapRemove() => Replay(new InsertDeleteGetRandomO1Solution.RandomizedSetByHashMapSwapRemove());

    private bool[] Replay(InsertDeleteGetRandomO1Solution.IRandomizedSet set)
    {
        var next = 0;

        foreach (var value in _insertOrder)
        {
            _verdicts[next++] = set.Insert(value);
        }

        foreach (var value in _removalOrder)
        {
            _verdicts[next++] = set.Remove(value);
        }

        return _verdicts;
    }
}
