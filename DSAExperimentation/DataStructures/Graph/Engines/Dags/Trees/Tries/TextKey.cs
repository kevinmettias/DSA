namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A string read as a trie key, its characters in order from the first: the key of every word trie.
// It holds the string and nothing else, so it travels in a register and the trie's walk reads each
// character straight from the string, bounded by the string's own length as a foreach over it is.
// It converts from a string implicitly, the way a string becomes a span, so a word trie's callers
// pass strings.
/// <summary>A string as a trie key, its characters read from the first.</summary>
/// <param name="text">The string the key reads.</param>
public readonly struct TextKey(string text) : ITrieKey<char>
{
    /// <inheritdoc/>
    public int Length => text.Length;

    /// <inheritdoc/>
    public char LetterAt(int position) => text[position];

    /// <summary>Reads a string as a key.</summary>
    /// <param name="text">The string to read.</param>
    /// <returns>The key of the string's characters.</returns>
    public static implicit operator TextKey(string text) => new(text);
}
