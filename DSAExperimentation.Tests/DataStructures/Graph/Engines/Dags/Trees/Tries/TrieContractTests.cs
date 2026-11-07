using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie's contract as a set of keys, written once over every kind of key and every payload.
// Each configuration that derives from this class (nested in TrieTests) names a trie to build, how
// to draw its keys and how to add and look one up, and inherits every fact: words over the slot
// store under both layouts and the hashed store, binary text over binary digits, each with the
// value payload, and an int's bits over the inline binary store with none, its presence implied by
// the path, beside TwoFieldBitTrie, the hand-specialized set for that layer. A trie may be a struct
// whose copies share one set, as TwoFieldBitTrie is, so each hook is handed the trie by value. A
// configuration spells each key it draws as text, independently of the key's own
// reading, so a key read in the wrong order, a store that loses a child or a payload that misreads
// a key's end answers differently from the model: a set of the spellings, whose prefixes are string
// prefixes. In the seeded script every answer must be the model's. What a map's value payload adds,
// a value under each key, is TrieTests' own.
public abstract partial class TrieContractTests<TTrie, TKey, TLetter>
    where TKey : struct, ITrieKey<TLetter>
{
    private const int ScriptLength = 2_000;
    private const int ScriptSeed = 208;
    private const int FirstSeed = 1;
    private const int SecondSeed = 2;

    [Fact]
    public void Add_ThenHasKey_IsTrue()
    {
        var trie = CreateTrie();
        var (key, _) = RandomKey(new Random(FirstSeed));
        Add(trie, key);

        Assert.True(HasKey(trie, key));
    }

    [Fact]
    public void Add_ExistingKey_KeepsTheCount()
    {
        var trie = CreateTrie();
        var (key, _) = RandomKey(new Random(FirstSeed));
        Add(trie, key);
        Add(trie, key);

        Assert.Equal(1, CountOf(trie));
    }

    [Fact]
    public void HasKey_EmptyTrie_IsFalse() => Assert.False(HasKey(CreateTrie(), RandomKey(new Random(FirstSeed)).Key));

    // Every prefix of a stored key is one, the whole key included.
    [Fact]
    public void HasPrefix_EveryLengthOfAStoredKey_IsTrue()
    {
        var trie = CreateTrie();
        var (key, _) = RandomKey(new Random(SecondSeed));
        Add(trie, key);

        Assert.All(Enumerable.Range(0, key.Length + 1), length => Assert.True(HasPrefix(trie, key, length)));
    }

    [Fact]
    public void HasPrefix_LengthZero_IsTrueOnlyOnceAKeyIsStored()
    {
        var trie = CreateTrie();
        var (key, _) = RandomKey(new Random(SecondSeed));
        var isPrefixOfEmptyTrie = HasPrefix(trie, key, 0);
        Add(trie, key);

        Assert.Equal((false, true), (isPrefixOfEmptyTrie, HasPrefix(trie, key, 0)));
    }

    [Fact]
    public void HasPrefix_LengthPastTheKey_Throws()
    {
        var (key, _) = RandomKey(new Random(SecondSeed));

        Assert.Throws<ArgumentOutOfRangeException>(() => HasPrefix(CreateTrie(), key, key.Length + 1));
    }

    [Fact]
    public void Count_CountsDistinctKeys()
    {
        var trie = CreateTrie();
        var random = new Random(ScriptSeed);
        var spellings = new HashSet<string>(StringComparer.Ordinal);

        for (var step = 0; step < ScriptLength; step++)
        {
            var (key, spelling) = RandomKey(random);
            Add(trie, key);
            spellings.Add(spelling);
        }

        Assert.Equal(spellings.Count, CountOf(trie));
    }

    // One seeded script of adds, each followed by lookups of a key drawn afresh, often not stored:
    // whether it is a key, and whether some stored key starts with its first letters, for a length
    // drawn from none of them to all.
    [Fact]
    public void SeededScript_AnswersAsASetOfTheSameKeysDoes()
    {
        var trie = CreateTrie();
        var model = new HashSet<string>(StringComparer.Ordinal);
        var random = new Random(ScriptSeed);

        for (var step = 0; step < ScriptLength; step++)
        {
            var (key, spelling) = RandomKey(random);
            Add(trie, key);
            model.Add(spelling);

            var probe = RandomKey(random);
            var length = random.Next(probe.Key.Length + 1);
            Assert.Equal(ModelAnswers(model, probe.Spelling, length), Answers(trie, probe.Key, length));
        }
    }

    // A new, empty trie of the configuration under test.
    protected abstract TTrie CreateTrie();

    // A key drawn for the configuration, with its letters spelled as text from the first.
    protected abstract (TKey Key, string Spelling) RandomKey(Random random);

    // Adds a key to the trie, as the configuration adds one.
    protected abstract void Add(TTrie trie, TKey key);

    // Whether the trie holds a key, as the configuration asks.
    protected abstract bool HasKey(TTrie trie, TKey key);

    // Whether some key the trie holds starts with a key's first letters, as the configuration asks.
    protected abstract bool HasPrefix(TTrie trie, TKey key, int length);

    // How many distinct keys the trie holds.
    protected abstract int CountOf(TTrie trie);

    private static (bool HasKey, bool HasPrefix, int Count) ModelAnswers(HashSet<string> model, string spelling, int length)
    {
        var prefix = spelling[..length];
        var hasPrefix = model.Any(stored => stored.StartsWith(prefix, StringComparison.Ordinal));

        return (model.Contains(spelling), hasPrefix, model.Count);
    }

    private (bool HasKey, bool HasPrefix, int Count) Answers(TTrie trie, TKey key, int length) =>
        (HasKey(trie, key), HasPrefix(trie, key, length), CountOf(trie));
}
