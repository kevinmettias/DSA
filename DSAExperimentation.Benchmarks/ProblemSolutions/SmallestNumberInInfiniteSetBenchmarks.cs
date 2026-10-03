using DSAExperimentation.LeetCode.SmallestNumberInInfiniteSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestNumberInInfiniteSetSolution's, the same factories
// SmallestNumberInInfiniteSetSolutionTests proves correct. [GlobalSetup] builds one fixed
// operation script - a PopSmallest per round, with an AddBack of an already-produced
// number interleaved roughly half the time so the added-back collection stays
// non-trivially populated instead of draining to empty every time - so script
// construction is charged to setup and only the replay is measured. The contrast is a
// List<int>'s O(n) minimum scan plus O(n) Contains per operation against
// Heap<Element, MinHeapOrder<Element>> + Set<Element>'s O(log n) pop and O(1) duplicate
// rejection. The script is CallCount calls long, up to LC 2336's 1,000 calls in total,
// which also keeps every added-back number inside its [1, 1000].
public class SmallestNumberInInfiniteSetBenchmarks
{
    private const int PopOpType = 0;
    private const int AddBackOpType = 1;
    private const int RandomSeed = 2336; // LC problem number
    private const int AddBackOneInEvery = 2;

    private (int Type, int Num)[] _ops = [];

    // What every PopSmallest returned, in script order - what each arm returns; an AddBack
    // returns nothing.
    private int[] _popped = [];

    [Params(100, 1_000)]
    public int CallCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var ops = new List<(int Type, int Num)>(CallCount);
        var producedSoFar = 0;

        // Every round adds a call, so the script reaches CallCount calls and stops there.
        while (ops.Count < CallCount)
        {
            ops.Add((PopOpType, 0));
            producedSoFar++;

            var mayAddBack = producedSoFar > 1 && ops.Count < CallCount;

            if (mayAddBack && random.Next(AddBackOneInEvery) == 0)
            {
                ops.Add((AddBackOpType, random.Next(1, producedSoFar)));
            }
        }

        _ops = [.. ops];
        _popped = new int[_ops.Count(op => op.Type == PopOpType)];
    }

    [Benchmark(Baseline = true)]
    public int[] ListScanPerOperation() => Replay(SmallestNumberInInfiniteSetSolution.CreateByListScan());

    [Benchmark]
    public int[] HeapAndSetPerOperation() => Replay(SmallestNumberInInfiniteSetSolution.CreateByHeapAndSet());

    private int[] Replay(SmallestNumberInInfiniteSetSolution.ISmallestInfiniteSet set)
    {
        var popCount = 0;

        foreach (var (type, num) in _ops)
        {
            if (type == AddBackOpType)
            {
                set.AddBack(num);
                continue;
            }

            _popped[popCount++] = set.PopSmallest();
        }

        return _popped;
    }
}
