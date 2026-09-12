using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AllOneDataStructure;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllOneDataStructureSolution's, the same factories
// AllOneDataStructureTests proves correct. [GlobalSetup] builds one fixed operation
// script - Length initial Inc calls seeding distinct keys, then Length rounds of a
// random-key Inc plus a GetMaxKey/GetMinKey pair - so script construction is charged to
// setup and only the replay is measured. A first version of this benchmark's composed
// arm grabbed a bucket's "any key" through a HashMap<string,bool>, whose Keys is an
// eager List snapshot of every entry - that mistake made the "primitive" approach lose
// by 5-10x, which is why BucketedLinkedListAllOne keeps a second, independent
// DoublyLinkedListNode chain per bucket instead.
[MemoryDiagnoser]
public class AllOneDataStructureBenchmarks
{
    private const int RandomSeed = 432; // LC problem number
    private const string KeyPrefix = "key";
    private const int OpsCapacityMultiplier = 4;
    private const int IncOpType = 0;
    private const int GetMaxKeyOpType = 2;
    private const int GetMinKeyOpType = 3;

    [Params(200, 5_000)]
    public int Length;

    private (int Type, string Key)[] _ops = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var keys = Enumerable.Range(0, Length).Select(i => KeyPrefix + i).ToArray();
        var ops = new List<(int Type, string Key)>(Length * OpsCapacityMultiplier);

        foreach (var key in keys)
        {
            ops.Add((IncOpType, key));
        }

        for (var round = 0; round < Length; round++)
        {
            ops.Add((IncOpType, keys[random.Next(keys.Length)]));
            ops.Add((GetMaxKeyOpType, ""));
            ops.Add((GetMinKeyOpType, ""));
        }

        _ops = [.. ops];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDictionaryScan() => Replay(AllOneDataStructureSolution.CreateByDictionaryScan());

    [Benchmark]
    public int BucketedLinkedListOnePass() => Replay(AllOneDataStructureSolution.CreateByBucketedLinkedList());

    private int Replay(AllOneDataStructureSolution.IAllOne allOne)
    {
        var checksum = 0;

        foreach (var (type, key) in _ops)
        {
            switch (type)
            {
                case IncOpType:
                    allOne.Inc(key);
                    break;
                case GetMaxKeyOpType:
                    checksum += allOne.GetMaxKey().Length;
                    break;
                case GetMinKeyOpType:
                    checksum += allOne.GetMinKey().Length;
                    break;
            }
        }

        return checksum;
    }
}
