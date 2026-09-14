using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestSubstringOfOneRepeatingCharacter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestSubstringOfOneRepeatingCharacterSolution's, the
// same methods LongestSubstringOfOneRepeatingCharacterTests proves correct. A raw
// char[] rescanned end to end after every point update (O(n) per query) against this
// repo's own SegmentTree<RunSegment,RunAggregate> (O(log n) Update, O(1) Query since
// every query spans the tree's full range). [GlobalSetup] generates the string and
// the query stream in LeetCode's own shape, so nothing but the reduction itself is
// charged to the measured methods.
[MemoryDiagnoser]
public class LongestSubstringOfOneRepeatingCharacterBenchmarks
{
    private const int RandomSeed = 2213; // LC problem number
    private const int AlphabetSize = 4; // small alphabet produces long runs, the case both strategies must handle well
    private const int QueryCount = 300;

    [Params(500, 5_000)]
    public int Length;

    private string _s = null!;
    private string _queryCharacters = null!;
    private int[] _queryIndices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

        var characters = new char[QueryCount];
        _queryIndices = new int[QueryCount];

        for (var i = 0; i < QueryCount; i++)
        {
            _queryIndices[i] = random.Next(0, Length);
            characters[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _queryCharacters = new string(characters);
    }

    [Benchmark(Baseline = true)]
    public int[] LinearRescanAfterEachUpdate() =>
        LongestSubstringOfOneRepeatingCharacterSolution.LongestRepeatingByLinearRescan(_s, _queryCharacters, _queryIndices);

    [Benchmark]
    public int[] SegmentTreeRunAggregate() =>
        LongestSubstringOfOneRepeatingCharacterSolution.LongestRepeatingBySegmentTree(_s, _queryCharacters, _queryIndices);
}
