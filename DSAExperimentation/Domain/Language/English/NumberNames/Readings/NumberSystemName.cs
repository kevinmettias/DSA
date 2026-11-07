using DSAExperimentation.Domain.Mathematics.NumberSystems;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Readings;

// A number system and the name a language calls it by: the reals are "real numbers".
/// <summary>A number system and its name.</summary>
/// <param name="System">The number system.</param>
/// <param name="Name">Its name, lowercase and plural.</param>
public readonly record struct NumberSystemName(NumberSystem System, string Name);
