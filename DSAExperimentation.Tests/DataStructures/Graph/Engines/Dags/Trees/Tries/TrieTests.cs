using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Domain.Science.Dna;
using DSAExperimentation.Tests.DataStructures.Alphabets.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// IPrefixMap's contract, run over every configuration of the one trie: the slot store under both
// layouts and the hashed store. Each fact holds for every configuration, and in each a seeded script of
// sets and lookups must answer as a dictionary of the same keys does, so a store that answers wrongly
// anywhere a map can see shows up here. What only one store promises is checked beside the contract: the characters each takes, and
// the slot store over other alphabets (case folded, a table alphabet with a letter out of code order,
// DNA's bases). The slot store's tree-topology witness is TrieTopologyTests'.
public sealed partial class TrieTests
{
    private const int ScriptLength = 2_000;
    private const int ScriptSeed = 208;
    private const int LongestScriptKey = 6;

    public static TheoryData<string> Configurations => [nameof(DenseTrieLayout), nameof(PackedTrieLayout), nameof(HashedChildren<int>)];

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Set_ThenTryGetValue_ReturnsTheStoredValue(string configuration)
    {
        var map = MapOf(configuration);
        map.Set("tree", 7);

        Assert.True(map.TryGetValue("tree", out var value));
        Assert.Equal(7, value);
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Set_ExistingKey_ReplacesTheValueAndKeepsTheCount(string configuration)
    {
        var map = MapOf(configuration);
        map.Set("tree", 1);
        map.Set("tree", 2);

        Assert.True(map.TryGetValue("tree", out var value));
        Assert.Equal((2, 1), (value, map.Count));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Set_EmptyKey_IsAKeyLikeAnyOther(string configuration)
    {
        var map = MapOf(configuration);
        map.Set(string.Empty, 3);

        Assert.True(map.HasKey(string.Empty));
        Assert.Equal(1, map.Count);
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasKey_PrefixOfAStoredKey_IsNoKey(string configuration)
    {
        var map = MapOf(configuration);
        map.Set("trees", 1);

        Assert.False(map.HasKey("tree"));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasKey_KeyThatIsAPrefixOfAnother_IsAKeyOfItsOwn(string configuration)
    {
        var map = MapOf(configuration);
        map.Set("trees", 1);
        map.Set("tree", 2);

        Assert.True(map.HasKey("tree") && map.HasKey("trees"));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void TryGetValue_UnknownKey_ReturnsFalse(string configuration) =>
        Assert.False(MapOf(configuration).TryGetValue("tree", out _));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasPrefix_PrefixOfAStoredKey_IsTrueAndAnyOtherFalse(string configuration)
    {
        var map = MapOf(configuration);
        map.Set("tree", 1);

        Assert.Equal((true, false), (map.HasPrefix("tr"), map.HasPrefix("ta")));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasPrefix_EmptyPrefix_IsTrueOnlyOnceAKeyIsStored(string configuration)
    {
        var map = MapOf(configuration);
        var isPrefixOfEmptyMap = map.HasPrefix(string.Empty);
        map.Set("a", 1);

        Assert.Equal((false, true), (isPrefixOfEmptyMap, map.HasPrefix(string.Empty)));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Count_CountsDistinctKeys(string configuration)
    {
        var map = MapOf(configuration);

        foreach (var key in new[] { "a", "ab", "a", "b", string.Empty })
        {
            map.Set(key, 1);
        }

        Assert.Equal(4, map.Count);
    }

    // One seeded script of sets over short lowercase keys, each followed by two lookups: a prefix of the
    // key just set, and a key drawn afresh, which is often no prefix of any key. Every answer must be the
    // one a dictionary of the keys set so far gives.
    [Theory]
    [MemberData(nameof(Configurations))]
    public void SeededScript_AnswersAsADictionaryOfTheSameKeysDoes(string configuration)
    {
        var map = MapOf(configuration);
        var model = new Dictionary<string, int>();
        var random = new Random(ScriptSeed);

        for (var step = 0; step < ScriptLength; step++)
        {
            var key = RandomKey(random);
            var value = random.Next();
            map.Set(key, value);
            model[key] = value;

            foreach (var probe in new[] { key[..random.Next(key.Length + 1)], RandomKey(random) })
            {
                Assert.Equal(ModelAnswers(model, probe), Answers(map, probe));
            }
        }
    }

    [Fact]
    public void HashedChildren_TakesAnyCharacter()
    {
        var map = new Trie<HashedChildren<int>, int>();
        map.Set("Été 2026!", 1);

        Assert.True(map.HasKey("Été 2026!"));
    }

    [Fact]
    public void AlphabetSlots_LetterOutsideTheAlphabet_IsRefused() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Trie<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>().Set("Tree", 1));

    [Fact]
    public void Root_IsTheSameNodeAcrossCalls()
    {
        var map = new Trie<HashedChildren<int>, int>();

        Assert.Same(map.Root, map.Root);
    }

    // "Cat" and "cat" are one key once case is folded, so the second Set overwrites the first, and a
    // lookup in a third spelling finds it.
    [Fact]
    public void Set_CaseFoldedAlphabet_StoresBothCasesUnderOneKey()
    {
        var trie = new Trie<AlphabetSlots<EnglishLetters, PackedTrieLayout, int>, int>();

        trie.Set("Cat", 1);
        trie.Set("cat", 2);
        trie.TryGetValue("cAT", out var value);

        Assert.Equal(2, value);
    }

    // ё is the seventh Russian letter but coded after я, so its slot comes from the alphabet's table:
    // "ёж" and "еж" differ only in it, and are two keys: setting the second leaves the first's value.
    [Fact]
    public void Set_TableAlphabet_KeepsALetterCodedOutOfOrderApart()
    {
        var trie = new Trie<AlphabetSlots<RussianLowercase, DenseTrieLayout, int>, int>();

        trie.Set("ёж", 1);
        trie.Set("еж", 2);
        trie.TryGetValue("ёж", out var value);

        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_DnaAlphabet_RefusesALetterThatIsNoBase()
    {
        var trie = new Trie<AlphabetSlots<DnaBases, PackedTrieLayout, int>, int>();

        trie.Set("ACGT", 1);

        Assert.True(trie.HasPrefix("ACG"));
        Assert.Throws<ArgumentOutOfRangeException>(() => trie.Set("ACGU", 1));
    }

    [Fact]
    public void Constructor_AlphabetOfMoreThan64Letters_ThrowsNotSupportedException() =>
        Assert.Throws<NotSupportedException>(() => new Trie<AlphabetSlots<SixtyFiveLetters, PackedTrieLayout, int>, int>());

    private static string RandomKey(Random random) =>
        new([.. Enumerable.Range(0, random.Next(LongestScriptKey)).Select(_ => EnglishLowercase.LetterAt(random.Next(EnglishLetters.Size)))]);

    // A prefix is a prefix of some stored key, so the empty prefix is one only once a key is stored.
    private static (bool HasKey, bool HasPrefix, int Value, int Count) ModelAnswers(Dictionary<string, int> model, string probe)
    {
        model.TryGetValue(probe, out var value);
        return (model.ContainsKey(probe), model.Keys.Any(key => key.StartsWith(probe, StringComparison.Ordinal)), value, model.Count);
    }

    private static (bool HasKey, bool HasPrefix, int Value, int Count) Answers(IPrefixMap<int> map, string probe)
    {
        map.TryGetValue(probe, out var value);
        return (map.HasKey(probe), map.HasPrefix(probe), value, map.Count);
    }

    private static IPrefixMap<int> MapOf(string configuration) => configuration switch
    {
        nameof(DenseTrieLayout) => new Trie<AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>, int>(),
        nameof(PackedTrieLayout) => new Trie<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>(),
        _ => new Trie<HashedChildren<int>, int>(),
    };
}
