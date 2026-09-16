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
    private const char KeyNumberPad = '0';
    private const int OpsCapacityMultiplier = 4;
    private const int IncOpType = 0;
    private const int GetMaxKeyOpType = 2;
    private const int GetMinKeyOpType = 3;

    private (int Type, string Key)[] _ops = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var keys = BuildKeys();
        var ops = new List<(int Type, string Key)>(Length * OpsCapacityMultiplier);

        AppendInitialIncs(ops, keys);
        AppendMixedRounds(ops, keys, random);

        _ops = [.. ops];
    }

    // The Length distinct keys the script seeds, zero-padded to one width so every generated key
    // has the same length. LC 432 pins GetMaxKey/GetMinKey only to return SOME key at the extreme
    // count, and the replay below sums the returned key's length - so with mixed-length names
    // ("key9" beside "key10") the value this benchmark returns would depend on which tied key an
    // arm happened to answer with. A bucket holds every key at a tied count, and the
    // dictionary-scan arm and the bucketed arm legitimately pick different members of it, so the
    // two arms were reporting values that were never comparable quantities. Padding removes the
    // tie-break from the answer without touching the script, the arms or the work either one does.
    private string[] BuildKeys()
    {
        var keyNumberWidth = Length.ToString().Length;

        return Enumerable.Range(0, Length)
            .Select(i => KeyPrefix + i.ToString().PadLeft(keyNumberWidth, KeyNumberPad))
            .ToArray();
    }

    // The Inc calls that seed every key at count 1, so the rounds that follow meet keys that
    // are already present and keys that are not.
    private static void AppendInitialIncs(List<(int Type, string Key)> ops, string[] keys)
    {
        foreach (var key in keys)
        {
            ops.Add((IncOpType, key));
        }
    }

    // The rounds the benchmark actually replays: one Inc on a key drawn from the whole seeded
    // range, then the GetMaxKey/GetMinKey pair whose returned keys the replay sums.
    private static void AppendMixedRounds(
        List<(int Type, string Key)> ops, string[] keys, Random random)
    {
        for (var round = 0; round < keys.Length; round++)
        {
            ops.Add((IncOpType, keys[random.Next(keys.Length)]));
            // Get* take no key argument; string.Empty is the absence, said once, rather
            // than the same two quote marks typed into both tuples.
            ops.Add((GetMaxKeyOpType, string.Empty));
            ops.Add((GetMinKeyOpType, string.Empty));
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDictionaryScan() => Replay(AllOneDataStructureSolution.CreateByDictionaryScan());

    [Benchmark]
    public int BucketedLinkedListOnePass() => Replay(AllOneDataStructureSolution.CreateByBucketedLinkedList());

    // Replay is shared by both [Benchmark] arms, so by the module's call-order rule it sits after
    // them rather than after the first arm that reaches it.
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
