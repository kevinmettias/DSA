namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The first letters of a key, read as a key of their own: the trie's prefix query walks it as it
// walks a whole key, so a prefix needs no copy of the letters it takes, and a key kind whose length
// is fixed, as an int's 32 bits are, still has prefixes of every length. A length outside the key
// is refused here, once, rather than left to read past the key: past a string's end that throws,
// but past an int's 32 bits a shift wraps round and would read a wrong letter silently.
/// <summary>The first letters of a trie key, as a key.</summary>
/// <typeparam name="TKey">The key whose first letters this takes.</typeparam>
/// <typeparam name="TLetter">One letter of the key.</typeparam>
public readonly struct KeyPrefix<TKey, TLetter> : ITrieKey<TLetter>
    where TKey : struct, ITrieKey<TLetter>
{
    private const string NotAPrefixLengthMessage = "Expected a prefix length from 0 to the key's length.";

    private readonly TKey _key;

    /// <inheritdoc/>
    public int Length { get; }

    /// <summary>Takes the first letters of a key.</summary>
    /// <param name="key">The key whose first letters to take.</param>
    /// <param name="length">How many letters to take, from 0 to the key's length.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative or more than the key's length.</exception>
    public KeyPrefix(TKey key, int length)
    {
        if ((uint)length > (uint)key.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, NotAPrefixLengthMessage);
        }

        _key = key;
        Length = length;
    }

    /// <inheritdoc/>
    public TLetter LetterAt(int position) => _key.LetterAt(position);
}
