namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// A cell's time is the earliest second it may be entered, and a walker cannot stand still: one that
// is early bounces between the cell it is on and a neighbour it can already reach, two seconds a
// round trip. Every second flips the parity of row + col, so an arrival short by an odd number of
// seconds waits one more than the shortfall to land on a valid parity.
//
// The bounce needs somewhere to bounce to. Every move but the first leaves a cell the walker just
// came from; the very first move has nothing behind it, so a caller whose start may have no
// enterable neighbour at second 1 checks that before searching.
internal readonly struct ParityBounceArrival : IArrivalRule
{
    // There and back: the bounce that covers an early arrival.
    private const int SecondsPerRoundTrip = 2;

    public static int Arrive(int departure, int cellTime)
    {
        var earliest = departure + 1;

        if (earliest >= cellTime)
        {
            return earliest;
        }

        var leftOverSecond = (cellTime - earliest) % SecondsPerRoundTrip;

        return cellTime + leftOverSecond;
    }
}
