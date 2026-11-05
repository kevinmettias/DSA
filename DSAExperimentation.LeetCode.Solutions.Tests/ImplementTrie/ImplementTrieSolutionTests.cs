using DSAExperimentation.LeetCode.Solutions.ImplementTrie;
using WordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        bool>,
    bool>;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ImplementTrie;

// Harness only. Both tries are ImplementTrieSolution's - this file replays LeetCode's
// own insert/search/startsWith call sequence against each, the same operation-script
// shape LRUCacheTests already uses for its own instance-API problem.
// ImplementTrieOp.Apply is pure dispatch onto each trie's own verbs - no trie logic of
// its own.
public sealed partial class ImplementTrieSolutionTests
{
    public static TheoryData<ImplementTrieOp[], bool?[]> Examples =>
        new()
        {
            {
                [
                    ImplementTrieOp.Insert("apple"),
                    ImplementTrieOp.Search("apple"),
                    ImplementTrieOp.Search("app"),
                    ImplementTrieOp.StartsWith("app"),
                    ImplementTrieOp.Insert("app"),
                    ImplementTrieOp.Search("app"),
                ],
                [null, true, false, true, null, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByArrayNodes_LeetCodeExample_MatchesExpectedBehavior(
        ImplementTrieOp[] operations, bool?[] expected)
    {
        var trie = ImplementTrieSolution.CreateByArrayNodes();

        Assert.Equal(expected, operations.Select(operation => operation.Apply(trie)));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTriePrimitive_LeetCodeExample_MatchesExpectedBehavior(
        ImplementTrieOp[] operations, bool?[] expected) =>
        Assert.Equal(expected, RunScript(ImplementTrieSolution.CreateByTriePrimitive(), operations));

    private static bool?[] RunScript(WordTrie trie, ImplementTrieOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(trie))];

    // One call in an Implement-Trie script: which method to invoke and on what word.
    // Built via the named factories below so a script (like Examples above) reads
    // like the LeetCode call sequence it replays. Nested here rather than left at
    // file scope so the file declares exactly one type.
    public readonly record struct ImplementTrieOp(ImplementTrieOp.OpKind kind, string word)
    {
        public static ImplementTrieOp Insert(string word) => new(OpKind.Insert, word);

        public static ImplementTrieOp Search(string word) => new(OpKind.Search, word);

        public static ImplementTrieOp StartsWith(string prefix) => new(OpKind.StartsWith, prefix);

        // null for insert (LeetCode's own void return), the boolean result for
        // search/startsWith - so a script runner can assert against one expected
        // value per operation uniformly. Internal, not public: only this same
        // assembly's test method ever calls Apply.
        internal bool? Apply(ImplementTrieSolution.ArrayNodeTrie trie)
        {
            if (kind == OpKind.Insert)
            {
                trie.Insert(word);
                return null;
            }

            if (kind == OpKind.Search)
            {
                return trie.HasWord(word);
            }

            return trie.HasPrefix(word);
        }

        internal bool? Apply(WordTrie trie)
        {
            if (kind == OpKind.Insert)
            {
                trie.Set(word, true);
                return null;
            }

            return kind == OpKind.Search ? trie.HasKey(word) : trie.HasPrefix(word);
        }

        public enum OpKind
        {
            Insert,
            Search,
            StartsWith,
        }
    }
}
