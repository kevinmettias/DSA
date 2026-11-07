namespace DSAExperimentation.Domain.Mathematics.Geometry;

// When side lengths close into a polygon on the flat (Euclidean) plane, as LC 976, LC 2971 and
// LC 3024 state it. This is a mathematical fact no data structure or algorithm represents, so it is
// Domain's (ARCHITECTURE §17.6). Sorting the sides and scanning for the largest perimeter stay
// with each strategy. LC 2971's and LC 3899's baselines ask this test; LC 976's and LC 3024's keep
// the three triangle inequalities, which compile tighter where the longest side is not at hand,
// and their tests hold them to it (§17.5).
//
// It is stated for the flat plane because it is the flat plane's: a sphere has two-sided polygons
// (lunes), and its side lengths are bounded by the sphere's size. On the flat plane, sides close
// into a polygon with positive area exactly when the longest is shorter than all the others
// together. For three sides that is the triangle inequality, so a triangle's test is this one asked
// of its longest side.
/// <summary>When side lengths close into a polygon on the flat plane.</summary>
public static class EuclideanPolygons
{
    /// <summary>The fewest sides a polygon on the flat plane has.</summary>
    public const int FewestSides = 3;

    /// <summary>
    /// Whether sides close into a polygon with positive area on the flat plane: the longest is
    /// shorter than all the others together.
    /// </summary>
    /// <param name="longestSide">The longest side's length.</param>
    /// <param name="otherSidesTotal">
    /// The other sides' lengths added up, at least <see cref="FewestSides"/> minus one of them; a long,
    /// because many sides can add up past an int.
    /// </param>
    /// <returns><see langword="true"/> when the longest side is the shorter; otherwise <see langword="false"/>.</returns>
    public static bool CanClose(int longestSide, long otherSidesTotal) => longestSide < otherSidesTotal;
}

// TODO: 
// LETS CREATE A PARENT CLASS FOR ALL FLAT SURFACES AND ANOTHER FOR NON-EUCLIDEAN ONES, WHERE THE ANGLES IS CALCULATED BASED ON GENERALIZED FORMULAS LIKE GAUSS–BONNET. 
// IF WE DO THAT THOUGH WE NEED A WAY TO COMPILE TIME FREEZE IT FOR PERFORMANCE REASONS.