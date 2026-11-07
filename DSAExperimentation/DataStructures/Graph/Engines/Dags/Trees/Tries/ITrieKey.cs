namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A key as the one trie walks it: a run of letters read from the first, each the label on one edge
// down from the root. What a letter is, and in what order a key's letters come, belongs to the key:
// a string's characters from the first (TextKey), or an int's bits from the most significant
// (Int32BitsKey). So Trie<TKey, TLetter, TStore, TValue> walks words and numbers with one loop, and
// each key kind is a struct the JIT compiles that loop for, its letters read inline. A key whose
// length is fixed, as an int's 32 bits are, answers Length with a constant.
/// <summary>A trie key: a sequence of letters, read from the first.</summary>
/// <typeparam name="TLetter">One letter, the label on one edge of the trie.</typeparam>
public interface ITrieKey<TLetter>
{
    /// <summary>Gets how many letters the key has.</summary>
    int Length { get; }

    /// <summary>Reads one letter of the key.</summary>
    /// <param name="position">The letter's place in the key, from 0 for the first to <see cref="Length"/> - 1.</param>
    /// <returns>The letter at that place.</returns>
    TLetter LetterAt(int position);
}
