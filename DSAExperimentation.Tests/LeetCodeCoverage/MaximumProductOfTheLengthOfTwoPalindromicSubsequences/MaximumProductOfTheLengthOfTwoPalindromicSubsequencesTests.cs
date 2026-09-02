using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductOfTheLengthOfTwoPalindromicSubsequences;

// LeetCode 2002. Maximum Product of the Length of Two Palindromic Subsequences:
// LeetCode caps s.Length at 12, so the intended solution genuinely is the 2^n
// subsequence space, not a shortcut around a smarter algorithm. Every subsequence
// (as an index bitmask) is enumerated via this repo's own Backtrack.Search - the
// same choose/explore/unchoose walk Subsets/SubsetsII already use - keeping only
// the ones that read as a palindrome, then the best product of two disjoint
// (non-overlapping bitmask) palindromic subsequences is taken via a plain pairwise
// scan.
public sealed partial class MaximumProductOfTheLengthOfTwoPalindromicSubsequencesTests
{
    [Fact]
    public void MaxProduct_LeetCodeExampleOne_ReturnsBestDisjointPalindromePairProduct()
    {
        var result = MaxProduct("leetcodecom");

        Assert.Equal(9, result);
    }

    [Fact]
    public void MaxProduct_LeetCodeExampleTwo_SplitsTheTwoCharactersEvenly()
    {
        var result = MaxProduct("bb");

        Assert.Equal(1, result);
    }

    [Fact]
    public void MaxProduct_LeetCodeExampleThree_ReturnsBestDisjointPalindromePairProduct()
    {
        var result = MaxProduct("accbcaxxcxx");

        Assert.Equal(25, result);
    }

    private static int MaxProduct(string s)
    {
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

    private sealed class SubsequenceState
    {
        public List<int> Indices { get; } = [];

        public int NextIndex { get; set; }
    }
}
