using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LongestCommonPrefixOfKStringsAfterRemoval;

// LeetCode 3485. Longest Common Prefix of K Strings After Removal: for every
// index i, remove words[i] and report the longest prefix shared by at least k of
// the remaining strings (0 if fewer than k remain).
//
// A prefix of length d is "shared by >= k strings" exactly when the trie node at
// depth d reached by that prefix has SubtreeWordCountAlgebra count >= k. Removing
// words[i] only ever decrements the count of nodes ON words[i]'s own root-to-leaf
// path, by exactly 1 each (its own multiplicity contribution) - every other node
// in the trie is untouched. So the deepest globally-qualifying depth (computed
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
    // longest length that ever reaches k. O(n * maxLength^2) - the arm the
    // composed trie strategy below has to beat.
    public static int[] AnswerByBruteForce(string[] words, int k)
    {
        var answer = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            answer[i] = LongestSharedPrefixExcluding(words, i, k);
        }

        return answer;
    }

    private static int LongestSharedPrefixExcluding(string[] words, int excludeIndex, int k)
    {
        if (words.Length - 1 < k)
        {
            return 0;
        }

        var maxLength = 0;

        for (var i = 0; i < words.Length; i++)
        {
            if (i != excludeIndex)
            {
                maxLength = Math.Max(maxLength, words[i].Length);
            }
        }

        var best = 0;

        for (var length = 1; length <= maxLength; length++)
        {
            if (MaxSharedPrefixCount(words, excludeIndex, length) >= k)
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
    public static int[] AnswerByReduceTrie(string[] words, int k)
    {
        var trie = BuildTrie(words);

        return AnswerByReduceTrie(trie, words, k);
    }

    public static int[] AnswerByReduceTrie(LowercaseTrie<int> trie, string[] words, int k)
    {
        var counts = Reduce.Tree<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>,
            DepthFirstReduceOrder<LowercaseTrieNode<int>>,
            SubtreeWordCountAlgebra, Dictionary<LowercaseTrieNode<int>, int>>(trie.Root);

        var depths = Reduce.Tree<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>,
            DepthFirstReduceOrder<LowercaseTrieNode<int>>,
            DistanceMapReduceAlgebra<LowercaseTrieNode<int>>, Dictionary<LowercaseTrieNode<int>, int>>(trie.Root);

        var maxLength = 0;

        foreach (var word in words)
        {
            maxLength = Math.Max(maxLength, word.Length);
        }

        // qualifyingNodeCount[d] = how many depth-d trie nodes have a prefix
        // count >= k, BEFORE any removal.
        var qualifyingNodeCount = new int[maxLength + 1];

        foreach (var (node, count) in counts)
        {
            if (count >= k)
            {
                qualifyingNodeCount[depths[node]]++;
            }
        }

        var deepestQualifyingDepth = -1;

        for (var depth = maxLength; depth >= 0; depth--)
        {
            if (qualifyingNodeCount[depth] > 0)
            {
                deepestQualifyingDepth = depth;
                break;
            }
        }

        var answer = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            answer[i] = AnswerFor(words[i], trie, counts, qualifyingNodeCount, deepestQualifyingDepth, k);
        }

        return answer;
    }

    // Depths beyond this word's own length are never on its trie path, so
    // removal can't touch them - the global deepest qualifying depth already
    // applies as-is whenever it lies past this word. Only when the word's own
    // path reaches (or exceeds) that depth does it need walking, from
    // deepestQualifyingDepth back toward the root, checking whether the one node
    // this word's removal can disqualify (its count dropping to exactly k - 1)
    // was the only thing keeping that depth qualified.
    private static int AnswerFor(
        string word,
        LowercaseTrie<int> trie,
        Dictionary<LowercaseTrieNode<int>, int> counts,
        int[] qualifyingNodeCount,
        int deepestQualifyingDepth,
        int k)
    {
        if (deepestQualifyingDepth > word.Length)
        {
            return deepestQualifyingDepth;
        }

        var path = WalkPath(trie, word);

        for (var depth = deepestQualifyingDepth; depth >= 0; depth--)
        {
            var dropped = counts[path[depth]] == k ? 1 : 0;

            if (qualifyingNodeCount[depth] - dropped > 0)
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
