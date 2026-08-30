using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Similar String Groups (LC 839): a naive "list of groups" merge - for every similar
// pair, linearly scan the current group list to find which group each index already
// belongs to (List<HashSet<int>>.First(g => g.Contains(...))) - vs. this repo's
// DisjointSet, whose Find resolves the same "which group is this index in" question
// in O(a(n)) amortized instead of O(currentGroupCount). Both variants run the
// identical O(n^2) pairwise similarity scan - deciding WHICH pairs to compare has no
// faster general strategy for this problem - so the group-membership lookup is the
// only axis left to compare, and it is a real one: whenever a pair turns out similar,
// naive pays for a linear scan through however many groups still exist, so its total
// cost grows with how many merges actually happen, not just with n^2. Words are
// generated as small clusters, each a shared base permutation of the same 8-letter
// alphabet with one fixed swap position pair per cluster, so real similar pairs - and
// therefore real merge work - actually occur instead of every word only ever being
// compared against strangers, the same "force genuine overlaps" intent
// AccountsMergeBenchmarks' shared email pool already uses.
[MemoryDiagnoser]
public class SimilarStringGroupsBenchmarks
{
    [Params(60, 240)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(839);
        const string alphabet = "abcdefgh";
        var clusterCount = Math.Max(1, WordCount / 8);

        var clusters = new (char[] Base, int SwapI, int SwapJ)[clusterCount];

        for (var c = 0; c < clusterCount; c++)
        {
            var chars = alphabet.ToCharArray();
            Shuffle(chars, random);

            var i = random.Next(chars.Length);
            int j;

            do
            {
                j = random.Next(chars.Length);
            } while (j == i);

            clusters[c] = (chars, i, j);
        }

        _words = new string[WordCount];

        for (var w = 0; w < WordCount; w++)
        {
            var (baseChars, i, j) = clusters[w % clusterCount];
            var word = (char[])baseChars.Clone();

            if (random.Next(2) == 0)
            {
                (word[i], word[j]) = (word[j], word[i]);
            }

            _words[w] = new string(word);
        }
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
    public int GroupListScan()
    {
        var groups = new List<HashSet<int>>();

        for (var i = 0; i < _words.Length; i++)
        {
            groups.Add([i]);
        }

        for (var i = 0; i < _words.Length; i++)
        {
            for (var j = i + 1; j < _words.Length; j++)
            {
                if (!IsSimilar(_words[i], _words[j]))
                {
                    continue;
                }

                var groupI = groups.First(g => g.Contains(i));
                var groupJ = groups.First(g => g.Contains(j));

                if (groupI != groupJ)
                {
                    groupI.UnionWith(groupJ);
                    groups.Remove(groupJ);
                }
            }
        }

        return groups.Count;
    }

    [Benchmark]
    public int DisjointSetByRank()
    {
        var components = new DisjointSet(_words.Length);

        for (var i = 0; i < _words.Length; i++)
        {
            for (var j = i + 1; j < _words.Length; j++)
            {
                if (IsSimilar(_words[i], _words[j]))
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new Set<int>();

        for (var i = 0; i < _words.Length; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count;
    }

    private static bool IsSimilar(string first, string second)
    {
        var mismatchCount = 0;
        var firstMismatch = -1;
        var secondMismatch = -1;

        for (var i = 0; i < first.Length; i++)
        {
            if (first[i] == second[i])
            {
                continue;
            }

            mismatchCount++;

            if (mismatchCount > 2)
            {
                return false;
            }

            if (mismatchCount == 1)
            {
                firstMismatch = i;
            }
            else
            {
                secondMismatch = i;
            }
        }

        return mismatchCount == 0
            || (mismatchCount == 2 && first[firstMismatch] == second[secondMismatch] && first[secondMismatch] == second[firstMismatch]);
    }
}
