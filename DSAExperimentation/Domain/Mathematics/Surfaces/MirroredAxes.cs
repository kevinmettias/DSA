namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// Which way a walker comes out reversed after crossing a twisted pair of edges. Crossing the twisted
// left and right edges of a Mobius strip turns the board upside down for the walker: its rows run
// the other way, so a step that went down now goes up, and left and right turns swap. A set of steps
// closed under that mirror (the orthogonal ones) is unchanged; an ordered one (a heading, a turning
// cycle) is not.
/// <summary>The axes along which a step that crossed a twisted edge comes out reversed.</summary>
[Flags]
public enum MirroredAxes
{
    /// <summary>Nothing reversed: no twisted edge was crossed, or one was crossed an even number of times.</summary>
    None = 0,

    /// <summary>The rows run the other way, as after crossing twisted left and right edges.</summary>
    Rows = 1,

    /// <summary>The columns run the other way, as after crossing twisted top and bottom edges.</summary>
    Columns = 2,
}
