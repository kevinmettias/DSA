using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.GroupsOfStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GroupsOfStringsSolution's, the same methods
// GroupsOfStringsTests proves correct - the O(n^2) pairwise popcount baseline
// against this repo's HashMap turning each word's O(26^2) delete/replace candidates
// into O(1) lookups. Both are handed the prepared letter-set masks their hoisted
// overload takes, so the mask pass is charged to [GlobalSetup] rather than to the
// grouping being measured.
//
// Each word is a random DISTINCT-letter subset of the alphabet (this problem's own
// precondition), so real add/delete/replace connections - not just strangers -
// actually occur.
[MemoryDiagnoser]
public class GroupsOfStringsBenchmarks
{
    private const int RandomSeed = 2157; // LC problem number
    private const int MinWordLength = 3;
    private const int MaxWordLengthExclusive = 9;
    private const int AlphabetSize = 26;

    private ArraySequence<int> _masks;

    [Params(200, 2_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var words = new string[WordCount];

        for (var i = 0; i < WordCount; i++)
        {
            words[i] = BuildRandomWord(random);
        }

        _masks = GroupsOfStringsSolution.LetterSetMasks(words);
    }

    private static string BuildRandomWord(Random random)
    {
        var alphabet = new char[AlphabetSize];

        for (var i = 0; i < AlphabetSize; i++)
        {
            alphabet[i] = (char)('a' + i);
        }

        Shuffle(alphabet, random);
        var length = random.Next(MinWordLength, MaxWordLengthExclusive);

        return new string(alphabet, 0, length);
    }

    private static void Shuffle(char[] chars, Random random)
    {
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public int[] PairwisePopCountScan() => GroupsOfStringsSolution.GroupSizesByPairwisePopCount(_masks);

    [Benchmark]
    public int[] HashMapNeighborLookup() => GroupsOfStringsSolution.GroupSizesByHashMapNeighbors(_masks);
}
