using DSAExperimentation.DataStructures.Trie;
using DSAExperimentation.LeetCode.ImplementTrie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementTrie;

// Harness only. The trie itself is DataStructures.Trie.Trie<bool> and the one
// strategy is ImplementTrieSolution's - this file replays LeetCode's own
// insert/search/startsWith call sequence against it, the same operation-script
// shape LRUCacheTests already uses for its own instance-API problem.
// ImplementTrieOp.Apply is pure dispatch onto Set/HasKey/HasPrefix - no trie logic
// of its own.
public sealed class ImplementTrieTests
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
    public void CreateByTriePrimitive_LeetCodeExample_MatchesExpectedBehavior(
        ImplementTrieOp[] operations, bool?[] expected) =>
        RunScript(ImplementTrieSolution.CreateByTriePrimitive(), operations, expected);

    private static void RunScript(Trie<bool> trie, ImplementTrieOp[] operations, bool?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(trie));
        }
    }
}

// One call in an Implement-Trie script: which method to invoke and on what word.
// Built via the named factories below so a script (like Examples above) reads
// like the LeetCode call sequence it replays.
public readonly record struct ImplementTrieOp
{
    private readonly Kind _kind;
    private readonly string _word;

    private ImplementTrieOp(Kind kind, string word)
    {
        _kind = kind;
        _word = word;
    }

    public static ImplementTrieOp Insert(string word) => new(Kind.Insert, word);

    public static ImplementTrieOp Search(string word) => new(Kind.Search, word);

    public static ImplementTrieOp StartsWith(string prefix) => new(Kind.StartsWith, prefix);

    // null for insert (LeetCode's own void return), the boolean result for
    // search/startsWith - so a script runner can assert against one expected
    // value per operation uniformly. Internal, not public: only this same
    // assembly's test method ever calls Apply.
    internal bool? Apply(Trie<bool> trie)
    {
        if (_kind == Kind.Insert)
        {
            trie.Set(_word, true);
            return null;
        }

        return _kind == Kind.Search ? trie.HasKey(_word) : trie.HasPrefix(_word);
    }

    private enum Kind
    {
        Insert,
        Search,
        StartsWith,
    }
}
