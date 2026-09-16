using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LongestCommonPrefixOfKStringsAfterRemoval;

// LeetCode 3485. Longest Common Prefix of K Strings After Removal: for every
// index i, remove words[i] and report the longest prefix shared by at least
// requiredShareCount of the remaining strings (0 if fewer than requiredShareCount
// remain).
//
// A prefix of length d is "shared by >= requiredShareCount strings" exactly when
// the trie node at depth d reached by that prefix has SubtreeWordCountAlgebra
// count >= requiredShareCount. Removing words[i] only ever decrements the count of
// nodes ON words[i]'s own root-to-leaf path, by exactly 1 each (its own multiplicity
// contribution) - every other node in the trie is untouched. So the deepest
// globally-qualifying depth (computed
// once, before any removal) is still exactly right for any word whose own length
// is shorter than it; only when a word's path reaches that depth does its removal
// need checking, and then only by walking that ONE path from the known deepest
// depth back toward the root; both facts keep the whole per-word pass at O(word
// length), summing to O(total input length) across all words.
internal static class LongestCommonPrefixOfKStringsAfterRemovalSolution
{
    // The textbook definition, unwound literally: for each removal, and for each
    // candidate length, count how many of the remaining strings share that exact
    // prefix (via a plain Dictionary<string,int>, no trie at all) and keep the
    // longest length that ever reaches requiredShareCount. O(n * maxLength^2) - the
    // arm the composed trie strategy below has to beat.
    public static int[] AnswerByBruteForce(string[] words, int requiredShareCount)
    {
        var answer = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            answer[i] = LongestSharedPrefixExcluding(words, i, requiredShareCount);
        }

        return answer;
    }

    private static int LongestSharedPrefixExcluding(string[] words, int excludeIndex, int requiredShareCount)
    {
        if (words.Length - 1 < requiredShareCount)
        {
            return 0;
        }

        var maxLength = LongestRemainingWordLength(words, excludeIndex);

        return LongestQualifyingLength(words, excludeIndex, requiredShareCount, maxLength);
    }

    // The longest word the removal actually leaves behind: the excluded index
    // contributes nothing, and that is what caps every candidate prefix length.
    private static int LongestRemainingWordLength(string[] words, int excludeIndex)
    {
        var maxLength = 0;

        for (var i = 0; i < words.Length; i++)
        {
            if (i != excludeIndex)
            {
                maxLength = Math.Max(maxLength, words[i].Length);
            }
        }

        return maxLength;
    }

    // The longest length still shared by requiredShareCount of the remaining words:
    // every candidate length is scored against the same exclusion, and the longest one
    // that clears requiredShareCount wins.
    private static int LongestQualifyingLength(string[] words, int excludeIndex, int requiredShareCount, int maxLength)
    {
        var best = 0;

        for (var length = 1; length <= maxLength; length++)
        {
            if (MaxSharedPrefixCount(words, excludeIndex, length) >= requiredShareCount)
            {
                best = length;
            }
        }

        return best;
    }

    private static int MaxSharedPrefixCount(string[] words, int excludeIndex, int length)
    {
        var counts = new Dictionary<string, int>();
        var best = 0;

        for (var i = 0; i < words.Length; i++)
        {
            if (i == excludeIndex || words[i].Length < length)
            {
                continue;
            }

            var prefix = words[i][..length];
            var count = counts.GetValueOrDefault(prefix) + 1;
            counts[prefix] = count;
            best = Math.Max(best, count);
        }

        return best;
    }

    // Composed: build the counting trie once, reduce it twice over the SAME
    // generic Tree engine LowercaseTrieTests already proves out (once with this
    // problem's own SubtreeWordCountAlgebra for per-node prefix multiplicity,
    // once with the repo's existing DistanceMapReduceAlgebra for per-node depth),
    // then answer every index with an O(word length) walk. O(total input length)
    // overall.
    public static int[] AnswerByReduceTrie(string[] words, int requiredShareCount)
    {
        var trie = BuildTrie(words);

        return AnswerByReduceTrie(trie, words, requiredShareCount);
    }

    public static int[] AnswerByReduceTrie(LowercaseTrie<int> trie, string[] words, int requiredShareCount)
    {
        var counts = ReduceSubtreeWordCounts(trie);
        var depths = ReduceNodeDepths(trie);
        var summary = BuildQualificationSummary(counts, depths, words, requiredShareCount);

        return AnswerEveryWord(trie, words, summary, requiredShareCount);
    }

    // Each node's own subtree word count, from this problem's SubtreeWordCountAlgebra.
    private static Dictionary<LowercaseTrieNode<int>, int> ReduceSubtreeWordCounts(LowercaseTrie<int> trie) =>
        Reduce.Tree<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>,
            DepthFirstReduceOrder<LowercaseTrieNode<int>>,
            SubtreeWordCountAlgebra, Dictionary<LowercaseTrieNode<int>, int>>(trie.Root);

    // Each node's depth from the root, from the repo's existing DistanceMapReduceAlgebra.
    private static Dictionary<LowercaseTrieNode<int>, int> ReduceNodeDepths(LowercaseTrie<int> trie) =>
        Reduce.Tree<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>,
            DepthFirstReduceOrder<LowercaseTrieNode<int>>,
            DistanceMapReduceAlgebra<LowercaseTrieNode<int>>, Dictionary<LowercaseTrieNode<int>, int>>(trie.Root);

    // The reduce pass folded down to the three facts AnswerFor needs, and stopped
    // there: the per-node counts themselves (AnswerFor indexes them by node), how
    // many depth-d nodes clear requiredShareCount at all, and the deepest depth that does.
    private static QualificationSummary BuildQualificationSummary(
        Dictionary<LowercaseTrieNode<int>, int> counts,
        Dictionary<LowercaseTrieNode<int>, int> depths,
        string[] words,
        int requiredShareCount)
    {
        var maxLength = LongestWordLength(words);
        var qualifyingNodeCount = CountQualifyingNodes(counts, depths, maxLength, requiredShareCount);
        var deepestQualifyingDepth = FindDeepestQualifyingDepth(qualifyingNodeCount);

        return new QualificationSummary(counts, qualifyingNodeCount, deepestQualifyingDepth);
    }

    // The longest word in the input, which caps how deep any prefix can reach. Written
    // as a loop rather than words.Max() so an empty input stays at 0 instead of throwing.
    private static int LongestWordLength(string[] words)
    {
        var maxLength = 0;

        foreach (var word in words)
        {
            maxLength = Math.Max(maxLength, word.Length);
        }

        return maxLength;
    }

    // qualifyingNodeCount[d] = how many depth-d trie nodes have a prefix
    // count >= requiredShareCount, BEFORE any removal.
    private static int[] CountQualifyingNodes(
        Dictionary<LowercaseTrieNode<int>, int> counts,
        Dictionary<LowercaseTrieNode<int>, int> depths,
        int maxLength,
        int requiredShareCount)
    {
        var qualifyingNodeCount = new int[maxLength + 1];

        foreach (var (node, count) in counts)
        {
            if (count >= requiredShareCount)
            {
                qualifyingNodeCount[depths[node]]++;
            }
        }

        return qualifyingNodeCount;
    }

    // The deepest depth with at least one qualifying node, or -1 when none qualifies
    // and only the empty prefix is left to answer with.
    private static int FindDeepestQualifyingDepth(int[] qualifyingNodeCount)
    {
        for (var depth = qualifyingNodeCount.Length - 1; depth >= 0; depth--)
        {
            if (qualifyingNodeCount[depth] > 0)
            {
                return depth;
            }
        }

        return -1;
    }

    // Every index answered in turn, each in O(word length), all from the one summary.
    private static int[] AnswerEveryWord(
        LowercaseTrie<int> trie, string[] words, QualificationSummary summary, int requiredShareCount)
    {
        var answer = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            answer[i] = AnswerFor(words[i], trie, summary, requiredShareCount);
        }

        return answer;
    }

    // Everything AnswerFor reads off the reduce pass, built once before the per-word
    // loop: each node's own subtree word count, how many depth-d nodes clear
    // requiredShareCount at all, and the deepest depth that does.
    private readonly record struct QualificationSummary(
        Dictionary<LowercaseTrieNode<int>, int> Counts,
        int[] QualifyingNodeCount,
        int DeepestQualifyingDepth);

    // Depths beyond this word's own length are never on its trie path, so
    // removal can't touch them - the global deepest qualifying depth already
    // applies as-is whenever it lies past this word. Only when the word's own
    // path reaches (or exceeds) that depth does it need walking, from
    // deepestQualifyingDepth back toward the root, checking whether the one node
    // this word's removal can disqualify (its count dropping to exactly
    // requiredShareCount - 1) was the only thing keeping that depth qualified.
    private static int AnswerFor(
        string word, LowercaseTrie<int> trie, QualificationSummary summary, int requiredShareCount)
    {
        if (summary.DeepestQualifyingDepth > word.Length)
        {
            return summary.DeepestQualifyingDepth;
        }

        var path = WalkPath(trie, word);

        for (var depth = summary.DeepestQualifyingDepth; depth >= 0; depth--)
        {
            var prefixCount = summary.Counts[path[depth]];
            var dropped = prefixCount == requiredShareCount ? 1 : 0;

            if (summary.QualifyingNodeCount[depth] - dropped > 0)
            {
                return depth;
            }
        }

        return 0;
    }

    // path[d] is the trie node reached after word's first d characters -
    // path[0] is the root, path[word.Length] is word's own end node.
    private static LowercaseTrieNode<int>[] WalkPath(LowercaseTrie<int> trie, string word)
    {
        var path = new LowercaseTrieNode<int>[word.Length + 1];
        var current = trie.Root;
        path[0] = current;

        for (var i = 0; i < word.Length; i++)
        {
            current = current.Children[word[i] - 'a']!;
            path[i + 1] = current;
        }

        return path;
    }

    // Every prefix depth's count is a node's own multiplicity (how many times
    // that EXACT string occurs in words) plus SubtreeWordCountAlgebra's summed
    // descendants, so duplicate words (LC's own "run","run","run" example) must
    // be tracked as a running count at the end node, not the plain
    // LowercaseTrie<bool> "is this a word" flag every other trie-backed solution
    // in this catalogue uses - that flag would silently collapse three "run"s
    // into one.
    public static LowercaseTrie<int> BuildTrie(string[] words)
    {
        var trie = new LowercaseTrie<int>();

        foreach (var word in words)
        {
            trie.TryGetValue(word, out var existing);
            trie.Set(word, existing + 1);
        }

        return trie;
    }
}
