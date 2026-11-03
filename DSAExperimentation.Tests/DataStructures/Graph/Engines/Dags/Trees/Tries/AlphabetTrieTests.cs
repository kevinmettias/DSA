using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Domain.Science.Dna;
using DSAExperimentation.Tests.DataStructures.Alphabets.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries.Fixtures;
using WordChildren = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrieChildren<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    int>;
using WordNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrieNode<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    int>;
using WordTopology = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrieTopology<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    int>;
using WordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrie<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    int>;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The trie's contract on the lowercase letters with packed nodes, the configuration most solutions
// pick; AlphabetTrieNodeTests proves the two layouts store children alike, and one fact here checks
// that whole tries built under each answer every lookup alike. The alphabet facts show the same trie
// on other alphabets: case folded, a table alphabet with a letter out of code order, and DNA's bases.
public sealed partial class AlphabetTrieTests
{
    public static TheoryData<string> LookupProbes => new()
    {
        Fixtures.EmptyPrefix, Fixtures.CarKey, Fixtures.CardKey, Fixtures.CatKey, Fixtures.SharedPrefixOfCarAndCat,
        Fixtures.DogKey, Fixtures.MissingKey, "z", "cardz",
    };

    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var trie = new WordTrie();

        trie.Set(Fixtures.CatKey, 1);
        var found = trie.TryGetValue(Fixtures.CatKey, out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue_CountUnchanged()
    {
        var trie = BuildTrie((Fixtures.CatKey, 1), (Fixtures.CatKey, Fixtures.OverwrittenValue));
        trie.TryGetValue(Fixtures.CatKey, out var value);

        Assert.Equal(Fixtures.OverwrittenValue, value);
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void Set_KeyOutsideTheAlphabet_ThrowsArgumentOutOfRangeException()
    {
        var trie = new WordTrie();

        Assert.Throws<ArgumentOutOfRangeException>(() => trie.Set(Fixtures.UppercaseCatKey, 1));
    }

    // "Cat" and "cat" are one key once case is folded, so the second Set overwrites the first, and a
    // lookup in a third spelling finds it.
    [Fact]
    public void Set_CaseFoldedAlphabet_StoresBothCasesUnderOneKey()
    {
        var trie = new AlphabetTrie<EnglishLetters, PackedTrieLayout, int>();

        trie.Set(Fixtures.UppercaseCatKey, 1);
        trie.Set(Fixtures.CatKey, Fixtures.OverwrittenValue);
        trie.TryGetValue("cAT", out var value);

        Assert.Equal(Fixtures.OverwrittenValue, value);
    }

    // ё is the seventh Russian letter but coded after я, so its slot comes from the alphabet's table:
    // "ёж" and "еж" differ only in it, and are two keys: setting the second leaves the first's value.
    [Fact]
    public void Set_TableAlphabet_KeepsALetterCodedOutOfOrderApart()
    {
        var trie = new AlphabetTrie<RussianLowercase, DenseTrieLayout, int>();

        trie.Set("ёж", 1);
        trie.Set("еж", Fixtures.OverwrittenValue);
        trie.TryGetValue("ёж", out var value);

        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_DnaAlphabet_RefusesALetterThatIsNoBase()
    {
        var trie = new AlphabetTrie<DnaBases, PackedTrieLayout, int>();

        trie.Set("ACGT", 1);

        Assert.True(trie.HasPrefix("ACG"));
        Assert.Throws<ArgumentOutOfRangeException>(() => trie.Set("ACGU", 1));
    }

    [Theory]
    [MemberData(nameof(LookupProbes))]
    public void Set_SameKeysUnderEitherLayout_AnswerEveryLookupAlike(string probe)
    {
        var dense = BuildTrie<DenseTrieLayout>();
        var packed = BuildTrie<PackedTrieLayout>();

        Assert.Equal(dense.HasKey(probe), packed.HasKey(probe));
        Assert.Equal(dense.HasPrefix(probe), packed.HasPrefix(probe));
        Assert.Equal(dense.TryGetValue(probe, out var denseValue), packed.TryGetValue(probe, out var packedValue));
        Assert.Equal(denseValue, packedValue);
    }

    [Fact]
    public void HasKey_UnknownKey_ReturnsFalse()
    {
        var trie = BuildTrie((Fixtures.CatKey, 1));

        Assert.True(trie.HasKey(Fixtures.CatKey));
        Assert.False(trie.HasKey(Fixtures.DogKey));
    }

    // "car" lies on the path "card" builds, but a longer key passing through its node does not make it
    // a key.
    [Fact]
    public void HasKey_PrefixOfAStoredKey_IsNoKeyUntilSet()
    {
        var cardOnly = BuildTrie((Fixtures.CardKey, Fixtures.CardValue));

        Assert.False(cardOnly.HasKey(Fixtures.CarKey));
    }

    // Setting "car" after "card" marks that interior node as a key of its own, and leaves "card" one.
    [Fact]
    public void HasKey_KeyThatIsAPrefixOfAnotherKey_IsMarkedIndependently()
    {
        var cardThenCar = BuildTrie((Fixtures.CardKey, Fixtures.CardValue), (Fixtures.CarKey, Fixtures.CarValue));

        Assert.True(cardThenCar.HasKey(Fixtures.CarKey));
        Assert.True(cardThenCar.HasKey(Fixtures.CardKey));
    }

    [Fact]
    public void TryGetValue_UnknownKey_ReturnsFalseAndDefault()
    {
        var trie = new WordTrie();

        var found = trie.TryGetValue(Fixtures.MissingKey, out var value);

        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void HasPrefix_KnownPrefix_ReturnsTrue()
    {
        var trie = BuildTrie((Fixtures.CardKey, 1));

        Assert.True(trie.HasPrefix(Fixtures.CarKey));
        Assert.True(trie.HasPrefix(Fixtures.CardKey));
    }

    [Fact]
    public void HasPrefix_UnknownPrefix_ReturnsFalse()
    {
        var trie = BuildTrie((Fixtures.CardKey, 1));

        Assert.False(trie.HasPrefix(Fixtures.DogKey));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsTrueWhenAnyKeyExists()
    {
        var trie = BuildTrie((Fixtures.CardKey, 1));

        Assert.True(trie.HasPrefix(Fixtures.EmptyPrefix));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsFalseWhenEmpty()
    {
        var trie = new WordTrie();

        Assert.False(trie.HasPrefix(Fixtures.EmptyPrefix));
    }

    [Fact]
    public void Count_TracksDistinctKeysOnly()
    {
        var trie = BuildTrie(
            (Fixtures.CatKey, 1),
            (Fixtures.CarKey, Fixtures.CarValue),
            (Fixtures.CardKey, Fixtures.CardValue),
            (Fixtures.CarKey, Fixtures.CarOverwrittenValue));

        Assert.Equal(Fixtures.ExpectedDistinctKeyCount, trie.Count);
    }

    [Fact]
    public void Root_ReturnsTheSameNodeAcrossCalls()
    {
        var trie = new WordTrie();

        Assert.Same(trie.Root, trie.Root);
    }

    [Fact]
    public void Constructor_AlphabetOfMoreThan64Letters_ThrowsNotSupportedException() =>
        Assert.Throws<NotSupportedException>(() => new AlphabetTrie<SixtyFiveLetters, PackedTrieLayout, int>());

    // Proves the ITreeTopology witness is real, not just a satisfied interface:
    // TreeMetrics.Size (Algorithms/Metrics, generic over ANY ITreeTopology) counts
    // every trie node reachable from the root with no trie-specific code at all -
    // root + c + a + r + d = 5 nodes for "car"/"card"'s shared-prefix chain.
    [Fact]
    public void Size_ViaTreeMetrics_CountsEveryNodeAcrossASharedPrefixChain()
    {
        var trie = BuildTrie((Fixtures.CarKey, 1), (Fixtures.CardKey, Fixtures.CardValueForSizeTest));

        var size = TreeMetrics.Size<WordNode, WordTopology, WordChildren>(trie.Root);

        Assert.Equal(Fixtures.ExpectedTreeSize, size);
    }

    // Proves the same witness generalizes to a SECOND generic engine, and that the
    // generalization is a genuinely useful one, not just a compiling one: Longest
    // Common Prefix of two keys IS LowestCommonAncestor.Find (Algorithms/Ancestry) over
    // a trie topology - the node where "car" and "cat" diverge is exactly the node
    // reached by walking their shared prefix "ca", with no trie-specific LCA code
    // required.
    [Fact]
    public void LowestCommonAncestor_OfTwoKeys_IsTheNodeAtTheirLongestCommonPrefix()
    {
        var trie = BuildTrie(
            (Fixtures.CarKey, 1),
            (Fixtures.CatKey, Fixtures.CatValueForLcaTest),
            (Fixtures.DogKey, Fixtures.DogValueForLcaTest));

        var carNode = WalkTo(trie.Root, Fixtures.CarKey);
        var catNode = WalkTo(trie.Root, Fixtures.CatKey);
        var sharedPrefixNode = WalkTo(trie.Root, Fixtures.SharedPrefixOfCarAndCat);

        var lca = LowestCommonAncestor.Find<WordNode, WordTopology, WordChildren>(trie.Root, carNode, catNode);

        Assert.Same(sharedPrefixNode, lca);
    }

    // The trie a fixture test starts from: the key/value Sets to replay, in order. One
    // builder rather than one per test, because every fixture in this file is the same
    // thing - a trie built by setting some keys - and the call site is where the keys
    // that matter to a particular test belong.
    private static WordTrie BuildTrie(params (string Key, int Value)[] entries)
    {
        var trie = new WordTrie();

        foreach (var (key, value) in entries)
        {
            trie.Set(key, value);
        }

        return trie;
    }

    // The same keys, in an order that adds children out of letter order at the root and at "ca",
    // under the layout asked for.
    private static AlphabetTrie<EnglishLowercase, TLayout, int> BuildTrie<TLayout>()
        where TLayout : struct, ITrieLayout
    {
        var trie = new AlphabetTrie<EnglishLowercase, TLayout, int>();

        trie.Set(Fixtures.DogKey, 1);
        trie.Set(Fixtures.CatKey, Fixtures.CatValueForLcaTest);
        trie.Set(Fixtures.CardKey, Fixtures.CardValue);
        trie.Set(Fixtures.CarKey, Fixtures.CarValue);

        return trie;
    }

    // Every key walked here is a prefix of a key this file has already inserted - "car",
    // "cat", or the "ca" the two share - so each child along the path was created by that
    // insert and is present by construction.
    private static WordNode WalkTo(WordNode root, string key)
    {
        var current = root;

        foreach (var ch in key)
        {
            current = current.ChildAt(EnglishLowercase.IndexOf(ch))
                ?? throw new InvalidOperationException($"WalkTo was given '{key}', which is not a prefix of any key in this trie");
        }

        return current;
    }

    /// <summary>
    /// The keys these tests insert, and the values and counts they expect back, named
    /// once so a second test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string CatKey = "cat";
        public const string DogKey = "dog";
        public const string CarKey = "car";
        public const string CardKey = "card";
        public const string MissingKey = "missing";
        public const string EmptyPrefix = "";
        public const string UppercaseCatKey = "Cat";
        public const string SharedPrefixOfCarAndCat = "ca";

        public const int OverwrittenValue = 2;
        public const int CarValue = 2;
        public const int CardValue = 3;
        public const int CarOverwrittenValue = 4;
        public const int ExpectedDistinctKeyCount = 3;
        public const int CardValueForSizeTest = 2;
        public const int ExpectedTreeSize = 5;
        public const int CatValueForLcaTest = 2;
        public const int DogValueForLcaTest = 3;
    }
}
