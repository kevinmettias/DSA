namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// A cell's time is when a move into it may begin, and a walker that is early waits where it stands:
// the move starts at the later of the two seconds and takes one.
/// <summary>An arrival rule where an early walker waits in place until a move into the cell may begin.</summary>
public readonly struct WaitInPlaceArrival : IArrivalRule
{
    public static int Arrive(int departure, int cellTime) => Math.Max(departure, cellTime) + 1;
}
