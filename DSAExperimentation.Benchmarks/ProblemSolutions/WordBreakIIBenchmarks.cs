using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Word Break II (LC 140): a plain HashSet-membership scan (every start index
// rescans candidate end positions all the way to s.Length, with no way to
// know a prefix is already dead) vs. this repo's own Trie<bool> + Memoizer,
// where Trie.HasPrefix lets the scan break out the moment no dictionary word
// starts with the current prefix. _s tiles a single short dictionary word so
// both strategies reach the identical unique sentence, isolating the
// segmentation-scan cost itself rather than sentence-construction cost.
[MemoryDiagnoser]
public class WordBreakIIBenchmarks
{
    private static readonly string[] Dictionary = ["cat"];

    [Params(600, 3000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup() => _s = string.Concat(Enumerable.Repeat("cat", Length / 3));

    [Benchmark(Baseline = true)]
    public int HashSetUnboundedScan()
    {
        var words = Dictionary.ToHashSet();
        var memo = new Dictionary<int, List<string>>();

        return From(0).Count;

        List<string> From(int start)
        {
            if (memo.TryGetValue(start, out var cached))
            {
                return cached;
            }

            if (start == _s.Length)
            {
                return [string.Empty];
            }

            var sentences = new List<string>();

            for (var end = start + 1; end <= _s.Length; end++)
            {
                var word = _s[start..end];

                if (!words.Contains(word))
                {
                    continue;
                }

                foreach (var suffix in From(end))
                {
                    sentences.Add(suffix.Length == 0 ? word : word + " " + suffix);
                }
            }

            return memo[start] = sentences;
        }
    }

    [Benchmark]
    public int TriePrunedMemoized()
    {
        var trie = new Trie<bool>();
        foreach (var word in Dictionary)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, List<string>>(0, From).Count;

        List<string> From(int start, Func<int, List<string>> from)
        {
            if (start == _s.Length)
            {
                return [string.Empty];
            }

            var sentences = new List<string>();

            for (var end = start + 1; end <= _s.Length; end++)
            {
                var piece = _s[start..end];

                if (!trie.HasPrefix(piece))
                {
                    break;
                }

                if (!trie.HasKey(piece))
                {
                    continue;
                }

                foreach (var suffix in from(end))
                {
                    sentences.Add(suffix.Length == 0 ? piece : piece + " " + suffix);
                }
            }

            return sentences;
        }
    }
}
