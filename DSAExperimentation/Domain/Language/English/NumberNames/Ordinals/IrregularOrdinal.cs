namespace DSAExperimentation.Domain.Language.English.NumberNames.Ordinals;

// A number word whose ordinal breaks the suffix rule: one gives first, not "oneth".
/// <summary>A number word and its irregular ordinal.</summary>
/// <param name="Cardinal">The number word.</param>
/// <param name="Ordinal">Its ordinal, lowercase.</param>
public readonly record struct IrregularOrdinal(NumberWord Cardinal, string Ordinal);
