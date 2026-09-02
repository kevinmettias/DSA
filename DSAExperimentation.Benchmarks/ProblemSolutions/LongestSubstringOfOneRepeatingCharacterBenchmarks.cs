using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Substring of One Repeating Character (LC 2213): a raw char[] rescanned
// end to end after every point update (O(n) per query) vs. this repo's own
// SegmentTree<RunSegment,RunAggregate> (O(log n) Update, O(1) Query since every
// query below spans the tree's full range). RunAggregate is duplicated from
// LongestSubstringOfOneRepeatingCharacterTests rather than shared - the same
// "Benchmarks project keeps its own copy of the solution" policy
// NumberOfLongestIncreasingSubsequenceBenchmarks' own doc comment states.
[MemoryDiagnoser]
public class LongestSubstringOfOneRepeatingCharacterBenchmarks
{
    private const int RandomSeed = 2213; // LC problem number
    private const int AlphabetSize = 4; // small alphabet produces long runs, the case both strategies must handle well
    private const int QueryCount = 300;

    [Params(500, 5_000)]
    public int Length;

    private char[] _initial = null!;
    private (int Index, char Character)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _initial = Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray();

        _queries = new (int Index, char Character)[QueryCount];
        for (var i = 0; i < QueryCount; i++)
        {
            _queries[i] = (random.Next(0, Length), (char)('a' + random.Next(AlphabetSize)));
        }
    }

    [Benchmark(Baseline = true)]
    public int[] LinearRescanAfterEachUpdate()
    {
        var chars = (char[])_initial.Clone();
        var lengths = new int[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            var (index, character) = _queries[i];
            chars[index] = character;
            lengths[i] = LongestRun(chars);
        }

        return lengths;
    }

    private static int LongestRun(char[] chars)
    {
        var best = 0;
        var i = 0;

        while (i < chars.Length)
        {
            var j = i;

            while (j < chars.Length && chars[j] == chars[i])
            {
                j++;
            }

            best = Math.Max(best, j - i);
            i = j;
        }

        return best;
    }

    [Benchmark]
    public int[] SegmentTreeRunAggregate()
    {
        var tree = new SegmentTree<RunSegment, RunAggregate>(BuildLeaves(_initial));
        var lengths = new int[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            var (index, character) = _queries[i];
            tree.Update(index, MakeLeaf(character));
            lengths[i] = tree.Query(0, _initial.Length - 1).MaxLen;
        }

        return lengths;
    }

    private static RunSegment[] BuildLeaves(char[] chars)
    {
        var leaves = new RunSegment[chars.Length];

        for (var i = 0; i < chars.Length; i++)
        {
            leaves[i] = MakeLeaf(chars[i]);
        }

        return leaves;
    }

    private static RunSegment MakeLeaf(char c) => new(c, c, Len: 1, PrefixLen: 1, SuffixLen: 1, MaxLen: 1);

    // See LongestSubstringOfOneRepeatingCharacterTests.RunSegment/RunAggregate for
    // the full explanation - repeated here rather than shared per this project's own
    // documented policy of not depending on the Tests project.
    private readonly record struct RunSegment(char Left, char Right, int Len, int PrefixLen, int SuffixLen, int MaxLen);

    private readonly struct RunAggregate : ICombineOperation<RunSegment>
    {
        public static RunSegment Identity => new('\0', '\0', Len: 0, PrefixLen: 0, SuffixLen: 0, MaxLen: 0);

        public static RunSegment Combine(RunSegment left, RunSegment right)
        {
            if (left.Len == 0)
            {
                return right;
            }

            if (right.Len == 0)
            {
                return left;
            }

            var bridges = left.Right == right.Left;
            var prefixLen = bridges && left.PrefixLen == left.Len ? left.Len + right.PrefixLen : left.PrefixLen;
            var suffixLen = bridges && right.SuffixLen == right.Len ? right.Len + left.SuffixLen : right.SuffixLen;
            var maxLen = Math.Max(left.MaxLen, right.MaxLen);

            if (bridges)
            {
                maxLen = Math.Max(maxLen, left.SuffixLen + right.PrefixLen);
            }

            return new RunSegment(left.Left, right.Right, left.Len + right.Len, prefixLen, suffixLen, maxLen);
        }
    }
}
