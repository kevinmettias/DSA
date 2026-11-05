using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.LeetCode.Solutions.ImplementTrie;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ImplementTrie;

// Harness only. Every strategy is ImplementTrieSolution's - this file replays LeetCode's
// own insert/search/startsWith call sequence against each, the same operation-script
// shape LRUCacheTests already uses for its own instance-API problem.
// ImplementTrieOp.Apply is pure dispatch onto IPrefixSet's verbs - no trie logic of its
// own. What every strategy owes beyond LeetCode's example is IPrefixSet's contract:
// ArrayNodeTrieTests holds the textbook trie to it, and PrefixSetTests the one trie.
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
        ImplementTrieOp[] operations, bool?[] expected) =>
        Assert.Equal(expected, RunScript(ImplementTrieSolution.CreateByArrayNodes(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPackedSlots_LeetCodeExample_MatchesExpectedBehavior(
        ImplementTrieOp[] operations, bool?[] expected) =>
        Assert.Equal(expected, RunScript(ImplementTrieSolution.CreateByPackedSlots(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByDenseSlots_LeetCodeExample_MatchesExpectedBehavior(
        ImplementTrieOp[] operations, bool?[] expected) =>
        Assert.Equal(expected, RunScript(ImplementTrieSolution.CreateByDenseSlots(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByHashedChildren_LeetCodeExample_MatchesExpectedBehavior(
        ImplementTrieOp[] operations, bool?[] expected) =>
        Assert.Equal(expected, RunScript(ImplementTrieSolution.CreateByHashedChildren(), operations));

    private static bool?[] RunScript(IPrefixSet words, ImplementTrieOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(words))];

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
        internal bool? Apply(IPrefixSet words)
        {
            if (kind == OpKind.Insert)
            {
                words.Add(word);
                return null;
            }

            return kind == OpKind.Search ? words.HasKey(word) : words.HasPrefix(word);
        }

        public enum OpKind
        {
            Insert,
            Search,
            StartsWith,
        }
    }
}
