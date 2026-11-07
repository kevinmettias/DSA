namespace DSAExperimentation.Domain.Mathematics.Groups;

// How many points the permutations of a symmetric group act on. It is a static abstract member of
// a struct, so SymmetricGroup<TDegree> reads it as a constant and makes its identity once per
// degree, as a board's rows and columns reach SlidingPuzzle<TBoard>.
/// <summary>The degree of a symmetric group: how many points its permutations act on.</summary>
public interface IPermutationDegree
{
    /// <summary>How many points the permutations act on, 0 or more.</summary>
    static abstract int Degree { get; }
}
