using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SimilarStringGroups;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SimilarStringGroupsSolution's, the same methods
// SimilarStringGroupsTests proves correct - the naive "list of index sets, linearly
// scanned for each merge" grouping vs. this repo's DisjointSet. Both run the
// identical O(n^2) pairwise similarity scan, so the group-membership lookup is the
// only axis being compared, and it is a real one: naive pays a linear scan through
// however many groups still exist every time a pair turns out similar, so its cost
// grows with how many merges actually happen.
//
// Words are generated as small clusters, each a shared base permutation of the same
// 8-letter alphabet with one fixed swap position pair per cluster, so real similar
// pairs - and therefore real merge work - actually occur instead of every word only
// ever being compared against strangers, the same "force genuine overlaps" intent
// AccountsMergeBenchmarks' shared email pool already uses. The generated pool is
// LeetCode's own string[] shape, so there is nothing to hoist beyond building it.
[MemoryDiagnoser]
public class SimilarStringGroupsBenchmarks
{
    private const int RandomSeed = 839; // LC 839
    private const int WordsPerCluster = 8;
    private const int CoinFlipBound = 2;
    private const string Alphabet = "abcdefgh";

    private string[] _words = [];

    [Params(60, 240)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var clusterCount = Math.Max(1, WordCount / WordsPerCluster);

        var clusters = new (char[] Base, int SwapI, int SwapJ)[clusterCount];

        for (var c = 0; c < clusterCount; c++)
        {
            clusters[c] = BuildCluster(Alphabet, random);
        }

        _words = new string[WordCount];

        for (var w = 0; w < WordCount; w++)
        {
            _words[w] = BuildWord(clusters, w, clusterCount, random);
        }
    }

    private static (char[] Base, int SwapI, int SwapJ) BuildCluster(string alphabet, Random random)
    {
        var chars = alphabet.ToCharArray();
        Shuffle(chars, random);

        var i = random.Next(chars.Length);
        int j;

        do
        {
            j = random.Next(chars.Length);
        } while (j == i);

        return (chars, i, j);
    }

    private static void Shuffle(char[] chars, Random random)
    {
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }

    private static string BuildWord(
        (char[] Base, int SwapI, int SwapJ)[] clusters, int wordIndex, int clusterCount, Random random)
    {
        var (baseChars, i, j) = clusters[wordIndex % clusterCount];
        var word = (char[])baseChars.Clone();

        if (random.Next(CoinFlipBound) == 0)
        {
            (word[i], word[j]) = (word[j], word[i]);
        }

        return new string(word);
    }

    [Benchmark(Baseline = true)]
    public int GroupListScan() => SimilarStringGroupsSolution.CountGroupsByGroupListScan(_words);

    [Benchmark]
    public int DisjointSetByRank() => SimilarStringGroupsSolution.CountGroupsByDisjointSet(_words);
}
