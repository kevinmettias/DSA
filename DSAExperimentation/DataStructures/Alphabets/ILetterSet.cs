using System.Collections.Immutable;

namespace DSAExperimentation.DataStructures.Alphabets;

// A named subset of an alphabet's letters, such as a language's vowels. Which letters belong is the
// language's fact, so a set's presets are Domain's; LetterSet reads them as a mask over the
// alphabet's slots. The set is tied to its alphabet in the type, so a vowel set cannot be asked
// about a slot of another alphabet. The letters are an array, the one source a set's count and any
// spelling of it are read from.
/// <summary>A named subset of an alphabet's letters.</summary>
/// <typeparam name="TAlphabet">The alphabet the letters belong to.</typeparam>
public interface ILetterSet<TAlphabet>
    where TAlphabet : struct, IAlphabet
{
    /// <summary>The set's letters, each once.</summary>
    static abstract ImmutableArray<char> Letters { get; }
}
