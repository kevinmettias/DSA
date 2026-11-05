namespace DSAExperimentation.Domain.Mathematics.Numerals;

// A way of writing a proportion as a count of parts out of a fixed whole: per cent counts hundredths,
// so 15% is 15 parts in 100, per mille thousandths, a basis point ten-thousandths, and parts per million
// millionths. The whole is the notation's one fact. Turning a count of parts into a share of an amount
// is left to the caller, because in floating point dividing by the whole last and multiplying by a rate
// worked out beforehand round differently, and a problem's arms must agree on which they do. The members
// are static abstract members of a struct, so the whole folds to a constant in a caller. The notations
// differ only in their whole, so they are this one contract with a preset for each, not one type that
// holds its whole in a field: as a type argument, each preset's whole is a constant, as a board type
// gives SlidingPuzzle its shape.
/// <summary>A notation that writes a proportion as parts of a fixed whole.</summary>
public interface IPartsPerNotation
{
    /// <summary>The number of parts that make up the whole, such as 100 for per cent.</summary>
    static abstract long PartsPerWhole { get; }
}
