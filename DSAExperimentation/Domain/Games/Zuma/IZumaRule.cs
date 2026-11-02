namespace DSAExperimentation.Domain.Games.Zuma;

// What makes a group of touching same-coloured balls vanish in a Zuma-style row: its length. Zuma
// itself clears three or more, other match games clear four, so the length is the rule's parameter
// and each game a configuration of it (ARCHITECTURE §17.6).
/// <summary>A Zuma-style collapse rule: how many touching balls of one colour vanish.</summary>
public interface IZumaRule
{
    /// <summary>The fewest touching balls of one colour that vanish; at least two.</summary>
    static abstract int ShortestVanishingGroup { get; }
}
