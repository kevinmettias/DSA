using DSAExperimentation.LeetCode.AllOneDataStructure;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllOneDataStructureSolution's, the same factories
// AllOneDataStructureSolutionTests proves correct. [GlobalSetup] builds one fixed operation
// script - Length initial Inc calls seeding distinct keys, then Length rounds of a
// random-key Inc plus a GetMaxKey/GetMinKey pair - so script construction is charged to
// setup and only the replay is measured. A first version of this benchmark's composed
// arm grabbed a bucket's "any key" through a HashMap<string,bool>, whose Keys is an
// eager List snapshot of every entry - that mistake made the "primitive" approach lose
// by 5-10x, which is why BucketedLinkedListAllOne keeps a second, independent
// DoublyLinkedListNode chain per bucket instead.
public class AllOneDataStructureBenchmarks
{
    private const int RandomSeed = 432; // LC problem number
    private const string KeyPrefix = "key";
    private const char FirstLetter = 'a';
    private const int AlphabetSize = 26;
    private const int OpsCapacityMultiplier = 4;
    private const int IncOpType = 0;
    private const int GetMaxKeyOpType = 2;
    private const int GetMinKeyOpType = 3;

    private (int Type, string Key)[] _ops = [];

    // Every key GetMaxKey/GetMinKey report, in replay order; sized in setup so the replay allocates nothing.
    private string[] _reported = [];

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
        _reported = new string[_ops.Count(op => op.Type != IncOpType)];
    }

    // The Length distinct keys the script seeds. LC 432's keys are lowercase letters only, so each
    // is "key" followed by its index spelled in base-26 letters ('a' standing for zero), padded to
    // one width so every key has the same length: six characters at 5,000 keys, inside LC 432's
    // ten. LC 432 pins GetMaxKey/GetMinKey only to return SOME key at the extreme count; a bucket
    // holds every key at a tied count, and the dictionary-scan arm and the bucketed arm
    // legitimately pick different members of it, which is why ArmAgreement lists this class among
    // the answers that differ by design.
    private string[] BuildKeys()
    {
        var letterWidth = LettersToSpell(Length);

        return Enumerable.Range(0, Length)
            .Select(i => KeyPrefix + SpellInLetters(i, letterWidth))
            .ToArray();
    }

    // How many base-26 letters it takes to give each of `count` indices its own spelling.
    private static int LettersToSpell(int count)
    {
        var width = 1;
        for (var spellable = AlphabetSize; spellable < count; spellable *= AlphabetSize)
        {
            width++;
        }

        return width;
    }

    // The index written in base 26, most significant letter first, padded with 'a' to `width` letters.
    private static string SpellInLetters(int index, int width)
    {
        var letters = new char[width];
        var remaining = index;
        for (var position = width - 1; position >= 0; position--)
        {
            letters[position] = (char)(FirstLetter + (remaining % AlphabetSize));
            remaining /= AlphabetSize;
        }

        return new string(letters);
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
    // range, then the GetMaxKey/GetMinKey pair whose returned keys the replay records.
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
    public string[] BruteForceDictionaryScan() => Replay(AllOneDataStructureSolution.CreateByDictionaryScan());

    [Benchmark]
    public string[] BucketedLinkedListOnePass() => Replay(AllOneDataStructureSolution.CreateByBucketedLinkedList());

    // Replay is shared by both [Benchmark] arms, so by the module's call-order rule it sits after
    // them rather than after the first arm that reaches it. It answers with every key the Get* calls
    // reported, in order.
    private string[] Replay(AllOneDataStructureSolution.IAllOne allOne)
    {
        var next = 0;

        foreach (var (type, key) in _ops)
        {
            switch (type)
            {
                case IncOpType:
                    allOne.Inc(key);
                    break;
                case GetMaxKeyOpType:
                    _reported[next++] = allOne.GetMaxKey();
                    break;
                case GetMinKeyOpType:
                    _reported[next++] = allOne.GetMinKey();
                    break;
            }
        }

        return _reported;
    }
}
