namespace DSAExperimentation.Domain.Mathematics.Modular;

// Arithmetic modulo m works in the residues 0 to m - 1, and is a fact of the integers, whichever
// algorithm computes with it: a sum or product reduced mod m is the sum or product of the reduced
// operands, reduced again. A modulus is a struct witness so a modulus that is a compile-time
// constant folds into the code that reads it, and a division by it becomes a multiplication.
//
// The repository's modular arithmetic multiplies two residues before reducing, so the product of
// the largest two, (m - 1)^2, must fit a long: m is at most 3,037,000,499.
/// <summary>A modulus: the number modular arithmetic wraps at.</summary>
public interface IModulus
{
    /// <summary>The modulus, from 2 up to 3,037,000,499 so that the product of two residues fits a long.</summary>
    static abstract long Value { get; }
}
