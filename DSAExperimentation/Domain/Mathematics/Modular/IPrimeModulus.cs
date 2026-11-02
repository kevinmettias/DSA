namespace DSAExperimentation.Domain.Mathematics.Modular;

// A prime modulus makes the residues a field: every residue but 0 has a multiplicative inverse, and
// by Fermat's little theorem it is a^(p - 2), since a^(p - 1) = 1 (mod p) for a not divisible by p.
// A composite modulus gives an inverse only to the residues coprime to it, and a^(m - 2) is not one
// in general, so the Fermat inverse is offered only where the modulus says it is prime.
/// <summary>A prime modulus, under which every nonzero residue has an inverse.</summary>
public interface IPrimeModulus : IModulus;
