namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// How a time-dependent grid search prices one move: the earliest second a walker leaving one cell
// at `departure` lands on a neighbour whose own time in the grid is `cellTime`. What that time means
// is the rule's to say - when the cell opens, or when a move into it may begin. A static-abstract
// witness (§12.4): every value the rule needs arrives through its signature, so each rule is fixed
// by type and GridEarliestArrival calls it directly.
//
// Laws GridEarliestArrival leans on: a move costs at least one second (arrival >= departure + 1),
// a rule is pure, and it is first-in-first-out over the departures a search actually produces at a
// cell - leaving later never lands earlier. ParityBounceArrival keeps that last law only within one
// parity, which is all a grid produces, since every move flips the parity of row + col.
/// <summary>How a time-dependent grid search prices one move into a neighbouring cell.</summary>
public interface IArrivalRule
{
    /// <summary>Computes the earliest second a walker leaving its cell can land on a neighbour.</summary>
    /// <param name="departure">The second the walker leaves its cell.</param>
    /// <param name="cellTime">
    /// The neighbour's own time in the grid; the rule decides whether that is when the cell opens or when a
    /// move into it may begin.
    /// </param>
    /// <returns>
    /// The landing second, at least <paramref name="departure"/> + 1, decided by the two arguments alone, and
    /// never earlier for a later departure the search makes from the same cell.
    /// </returns>
    static abstract int Arrive(int departure, int cellTime);
}
