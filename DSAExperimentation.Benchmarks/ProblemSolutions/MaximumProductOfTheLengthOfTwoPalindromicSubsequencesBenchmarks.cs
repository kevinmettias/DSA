using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Product of the Length of Two Palindromic Subsequences (LC 2002): LeetCode
// caps s.Length at 12, so the intended solution is genuinely exponential. This
// benchmark compares hand-rolled bitmask enumeration (BruteForce) against this
// repo's own Backtrack.Search choose/explore/unchoose walk (Backtracking) for
// generating the identical 2^n subsequence space, both followed by the same
// disjoint-pair product scan.
[MemoryDiagnoser]
public class MaximumProductOfTheLengthOfTwoPalindromicSubsequencesBenchmarks
{
    private const int AlphabetSize = 4; // characters are drawn from 'a'-'d'

    [Params(8, 12)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _value = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var s = _value;
        var totalMasks = 1 << s.Length;
        var palindromes = new List<(int Mask, int Length)>();

        for (var mask = 1; mask < totalMasks; mask++)
        {
            if (!IsPalindromicMask(s, mask))
            {
                continue;
            }

            palindromes.Add((mask, BitCount(mask)));
        }

        return BestDisjointProduct(palindromes);
    }

    [Benchmark]
    public int Backtracking()
    {
        var s = _value;
        var palindromes = new List<(int Mask, int Length)>();
        var state = new SubsequenceState();

        Backtrack.Search<SubsequenceState, int>(
            state,
            isSolution: static _ => true,
            candidates: st => Enumerable.Range(st.NextIndex, s.Length - st.NextIndex),
            choose: (st, index) =>
            {
                st.Indices.Add(index);
                st.NextIndex = index + 1;
            },
            unchoose: (st, _) => st.Indices.RemoveAt(st.Indices.Count - 1),
            onSolution: st => RecordIfPalindromic(s, st, palindromes));

        return BestDisjointProduct(palindromes);
    }

    private static void RecordIfPalindromic(string s, SubsequenceState state, List<(int Mask, int Length)> palindromes)
    {
        if (state.Indices.Count == 0 || !IsPalindromicSubsequence(s, state.Indices))
        {
            return;
        }

        var mask = 0;
        foreach (var index in state.Indices)
        {
            mask |= 1 << index;
        }

        palindromes.Add((mask, state.Indices.Count));
    }

    private static int BestDisjointProduct(List<(int Mask, int Length)> palindromes)
    {
        var best = 0;

        for (var i = 0; i < palindromes.Count; i++)
        {
            for (var j = i + 1; j < palindromes.Count; j++)
            {
                if ((palindromes[i].Mask & palindromes[j].Mask) == 0)
                {
                    best = Math.Max(best, palindromes[i].Length * palindromes[j].Length);
                }
            }
        }

        return best;
    }

    private static bool IsPalindromicMask(string s, int mask)
    {
        var indices = new List<int>();

        for (var i = 0; i < s.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                indices.Add(i);
            }
        }

        return IsPalindromicSubsequence(s, indices);
    }

    private static bool IsPalindromicSubsequence(string s, List<int> indices)
    {
        var left = 0;
        var right = indices.Count - 1;

        while (left < right)
        {
            if (s[indices[left]] != s[indices[right]])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    private static int BitCount(int mask)
    {
        var count = 0;

        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }

    private sealed class SubsequenceState
    {
        public List<int> Indices { get; } = [];

        public int NextIndex { get; set; }
    }
}
