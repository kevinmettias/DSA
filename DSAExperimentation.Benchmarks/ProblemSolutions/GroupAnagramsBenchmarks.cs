using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GroupAnagrams;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GroupAnagramsSolution's, the same methods
// GroupAnagramsTests proves correct. Each arm reports the built group count
// rather than the groups themselves, so the result isn't discarded as dead
// code without materializing a potentially large object graph on every
// iteration.
[MemoryDiagnoser]
public class GroupAnagramsBenchmarks
{
    private const int SourceWordCount = 2;
    private const string FirstAnagramWord = "eat";
    private const string SecondAnagramWord = "tea";

    private string[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
        => _values = Enumerable.Range(0, Length)
            .Select(index => IsFirstWord(index) ? FirstAnagramWord : SecondAnagramWord)
            .ToArray();

    private static bool IsFirstWord(int index) => index % SourceWordCount == 0;

    [Benchmark(Baseline = true)]
    public int DictionaryGroup() => GroupAnagramsSolution.GroupByDictionary(_values).Count;

    [Benchmark]
    public int HashMapGroup() => GroupAnagramsSolution.GroupByHashMap(_values).Count;
}
