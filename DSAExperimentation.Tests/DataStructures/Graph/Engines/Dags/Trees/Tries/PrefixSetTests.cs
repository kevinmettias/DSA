using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// IPrefixSet's contract, written once and run over every set that implements it. Each class that
// derives from this one names a set to build, and inherits every fact: here the one trie's set view
// under both slot layouts and the hashed store, and in DSAExperimentation.LeetCode.Solutions.Tests
// LC 208's hand-written trie, the strategy that specializes this layer (ARCHITECTURE §17.5). In each,
// a seeded script of adds and lookups must answer as a HashSet of the same keys does. The keys are
// lowercase English words, which every one of those sets takes.
public abstract partial class PrefixSetTests
{
    private const int ScriptLength = 2_000;
    private const int ScriptSeed = 208;
    private const int LongestScriptKey = 6;

    [Fact]
    public void Add_ThenHasKey_IsTrue()
    {
        var set = CreateSet();
        set.Add("tree");

        Assert.True(set.HasKey("tree"));
    }

    [Fact]
    public void Add_ExistingKey_KeepsTheCount()
    {
        var set = CreateSet();
        set.Add("tree");
        set.Add("tree");

        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void Add_EmptyKey_IsAKeyLikeAnyOther()
    {
        var set = CreateSet();
        set.Add(string.Empty);

        Assert.Equal((true, 1), (set.HasKey(string.Empty), set.Count));
    }

    [Fact]
    public void HasKey_PrefixOfAStoredKey_IsNoKey()
    {
        var set = CreateSet();
        set.Add("trees");

        Assert.False(set.HasKey("tree"));
    }

    [Fact]
    public void HasKey_KeyThatIsAPrefixOfAnother_IsAKeyOfItsOwn()
    {
        var set = CreateSet();
        set.Add("trees");
        set.Add("tree");

        Assert.True(set.HasKey("tree") && set.HasKey("trees"));
    }

    // "trees" runs past the only key's last letter, so no stored key starts with it.
    [Fact]
    public void HasPrefix_PrefixOfAStoredKey_IsTrueAndAnyOtherFalse()
    {
        var set = CreateSet();
        set.Add("tree");

        Assert.Equal((true, false, false), (set.HasPrefix("tr"), set.HasPrefix("ta"), set.HasPrefix("trees")));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_IsTrueOnlyOnceAKeyIsStored()
    {
        var set = CreateSet();
        var isPrefixOfEmptySet = set.HasPrefix(string.Empty);
        set.Add("a");

        Assert.Equal((false, true), (isPrefixOfEmptySet, set.HasPrefix(string.Empty)));
    }

    [Fact]
    public void Count_CountsDistinctKeys()
    {
        var set = CreateSet();

        foreach (var key in new[] { "a", "ab", "a", "b", string.Empty })
        {
            set.Add(key);
        }

        Assert.Equal(4, set.Count);
    }

    // One seeded script of adds over short keys, each followed by two lookups: a prefix of the key just
    // added, and a key drawn afresh, which is often no prefix of any key. Every answer must be the one a
    // HashSet of the keys added so far gives.
    [Fact]
    public void SeededScript_AnswersAsAHashSetOfTheSameKeysDoes()
    {
        var set = CreateSet();
        var model = new HashSet<string>(StringComparer.Ordinal);
        var random = new Random(ScriptSeed);

        for (var step = 0; step < ScriptLength; step++)
        {
            var key = RandomKey(random);
            set.Add(key);
            model.Add(key);

            foreach (var probe in new[] { key[..random.Next(key.Length + 1)], RandomKey(random) })
            {
                Assert.Equal(ModelAnswers(model, probe), Answers(set, probe));
            }
        }
    }

    // A new, empty set to hold to the contract.
    protected abstract IPrefixSet CreateSet();

    private static string RandomKey(Random random) =>
        new([.. Enumerable.Range(0, random.Next(LongestScriptKey)).Select(_ => EnglishLowercase.LetterAt(random.Next(EnglishLetters.Size)))]);

    // A prefix is a prefix of some stored key, so the empty prefix is one only once a key is stored.
    private static (bool HasKey, bool HasPrefix, int Count) ModelAnswers(HashSet<string> model, string probe) =>
        (model.Contains(probe), model.Any(key => key.StartsWith(probe, StringComparison.Ordinal)), model.Count);

    private static (bool HasKey, bool HasPrefix, int Count) Answers(IPrefixSet set, string probe) =>
        (set.HasKey(probe), set.HasPrefix(probe), set.Count);

    public sealed class OverDenseSlots : PrefixSetTests
    {
        protected override IPrefixSet CreateSet() =>
            new PrefixSet<AlphabetSlots<EnglishLowercase, DenseTrieLayout, bool>>(new());
    }

    public sealed class OverPackedSlots : PrefixSetTests
    {
        protected override IPrefixSet CreateSet() =>
            new PrefixSet<AlphabetSlots<EnglishLowercase, PackedTrieLayout, bool>>(new());
    }

    public sealed class OverHashedChildren : PrefixSetTests
    {
        protected override IPrefixSet CreateSet() => new PrefixSet<HashedChildren<bool>>(new());
    }
}
