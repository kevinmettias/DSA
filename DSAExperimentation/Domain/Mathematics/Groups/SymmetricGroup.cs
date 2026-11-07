using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.Domain.Mathematics.Groups;

// The symmetric group S_n: every permutation of n points, combined by doing one after the other.
// IMonoid's Identity is static, and a permutation's identity depends on its degree, so the group
// needs n fixed per type: TDegree states it as a constant, and the identity is made once per degree
// and shared, which an immutable permutation allows. Permutation itself works at a degree chosen at
// run time and is this group's element type.
//
// Combine(left, right) applies left first, then right: right.After(left), right ∘ left in function
// notation. That is IMonoid's "left first" read as time, and it is how computer algebra (GAP) and
// the Rubik's cube's move sequences multiply, "R U" turning R first. A structure that combines an
// earlier element on the left, as a segment tree combines ranges, then composes a run of moves in
// the order they were made.
//
// A permutation of another degree is not in the group, and Combine and Invert refuse it. The check
// compares a degree with TDegree's constant.
/// <summary>The symmetric group of a degree: its permutations, composed left first.</summary>
/// <typeparam name="TDegree">How many points the permutations act on.</typeparam>
public readonly struct SymmetricGroup<TDegree> : IGroup<Permutation>
    where TDegree : struct, IPermutationDegree
{
    private const string OutsideTheGroup = "The permutation does not act on the group's degree.";

    private static readonly Permutation IdentityOfDegree = Permutation.Identity(TDegree.Degree);

    /// <summary>The identity of the group's degree, which sends every point to itself.</summary>
    public static Permutation Identity => IdentityOfDegree;

    /// <summary>Composes two permutations, the left one applied first.</summary>
    /// <param name="left">The permutation applied first.</param>
    /// <param name="right">The permutation applied second.</param>
    /// <returns>The composite, right ∘ left, which sends i to right's image of left(i).</returns>
    /// <exception cref="ArgumentException">Either permutation is of another degree.</exception>
    public static Permutation Combine(Permutation left, Permutation right)
    {
        CheckDegree(left, nameof(left));
        CheckDegree(right, nameof(right));
        return right.After(left);
    }

    /// <summary>Finds a permutation's inverse in the group.</summary>
    /// <param name="value">The permutation.</param>
    /// <returns>The permutation that sends each image back to its point.</returns>
    /// <exception cref="ArgumentException">The permutation is of another degree.</exception>
    public static Permutation Invert(Permutation value)
    {
        CheckDegree(value, nameof(value));
        return value.Inverse();
    }

    // Refuses a permutation of another degree, which is not an element of this group.
    private static void CheckDegree(Permutation value, string name)
    {
        if (value.Degree != TDegree.Degree)
        {
            throw new ArgumentException(OutsideTheGroup, name);
        }
    }
}
