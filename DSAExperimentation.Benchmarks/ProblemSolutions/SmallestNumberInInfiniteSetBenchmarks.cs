using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestNumberInInfiniteSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestNumberInInfiniteSetSolution's, the same factories
// SmallestNumberInInfiniteSetTests proves correct. [GlobalSetup] builds one fixed
// operation script - a PopSmallest per round, with an AddBack of an already-produced
// number interleaved roughly half the time so the added-back collection stays
// non-trivially populated instead of draining to empty every time - so script
// construction is charged to setup and only the replay is measured. The contrast is a
// List<int>'s O(n) minimum scan plus O(n) Contains per operation against
// Heap<Element, MinHeapOrder<Element>> + Set<Element>'s O(log n) pop and O(1) duplicate
// rejection.
[MemoryDiagnoser]
public class SmallestNumberInInfiniteSetBenchmarks
{
    private const int OpsCapacityMultiplier = 2;
    private const int PopOpType = 0;
    private const int AddBackOpType = 1;
    private const int RandomSeed = 2336; // LC problem number
    private const int AddBackOneInEvery = 2;

    private (int Type, int Num)[] _ops = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var ops = new List<(int Type, int Num)>(Length * OpsCapacityMultiplier);
        var producedSoFar = 0;

        for (var i = 0; i < Length; i++)
        {
            ops.Add((PopOpType, 0));
            producedSoFar++;

            if (producedSoFar > 1 && random.Next(AddBackOneInEvery) == 0)
            {
                ops.Add((AddBackOpType, random.Next(1, producedSoFar)));
            }
        }

        _ops = [.. ops];
    }

    [Benchmark(Baseline = true)]
    public long ListScanPerOperation() => Replay(SmallestNumberInInfiniteSetSolution.CreateByListScan());

    [Benchmark]
    public long HeapAndSetPerOperation() => Replay(SmallestNumberInInfiniteSetSolution.CreateByHeapAndSet());

    private long Replay(SmallestNumberInInfiniteSetSolution.ISmallestInfiniteSet set)
    {
        var checksum = 0L;

        foreach (var (type, num) in _ops)
        {
            if (type == AddBackOpType)
            {
                set.AddBack(num);
                continue;
            }

            checksum += set.PopSmallest();
        }

        return checksum;
    }
}
