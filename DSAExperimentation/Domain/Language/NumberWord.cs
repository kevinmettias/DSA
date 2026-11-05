namespace DSAExperimentation.Domain.Language;

// A number and the word a language names it with: seventeen is 17, a million 1,000,000. The number is
// the one source a table of words is built from, so a table places each word at its own number and
// cannot disagree with it.
/// <summary>A number and the word that names it.</summary>
/// <param name="Value">The number the word names.</param>
/// <param name="Word">The word, lowercase, as it is written within a sentence.</param>
public readonly record struct NumberWord(long Value, string Word);
