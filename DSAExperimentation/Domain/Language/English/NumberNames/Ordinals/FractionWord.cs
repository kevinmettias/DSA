namespace DSAExperimentation.Domain.Language.English.NumberNames.Ordinals;

// A denominator with a fraction word of its own rather than its ordinal: two gives half, not
// "second", and halves, not "halfs".
/// <summary>A denominator and its own fraction word, singular and plural.</summary>
/// <param name="Denominator">The denominator's number word.</param>
/// <param name="Singular">The word for one part, such as half.</param>
/// <param name="Plural">The word for several parts, such as halves.</param>
public readonly record struct FractionWord(NumberWord Denominator, string Singular, string Plural);
