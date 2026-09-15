namespace DSAExperimentation.LeetCode.MirrorReflection;

// LeetCode 858. Mirror Reflection: a laser fired from the south-west corner of a
// square room of side p at height q on the east wall, reported as the receptor it
// eventually hits. Unfolding the reflections turns the bouncing ray into a straight
// line through a stack of copies of the room, so the answer is decided by the
// parity of how many rooms it crosses horizontally (k) and vertically (m) before
// landing on a corner.
//
// ReceptorBySimulatedUnfolding walks k upward one room at a time until k*q is
// divisible by p - the textbook O(p) simulation. ReceptorByGcdReduction reaches the
// same k directly: k = p / gcd(p, q) and m = q / gcd(p, q), so reducing the pair by
// its GCD (the Euclidean algorithm, this repo's inline-primitive precedent from
// MaxPointsOnALine's slope reduction and ReachingPoints' backward reduction) and
// reading the reduced pair's parity is O(log(min(p, q))). No repo container or
// algorithm primitive applies to a single running (p, q) pair - the same "lighter
// repo-primitive fit" category ReachingPoints and Pow(x, n) already establish.
internal static class MirrorReflectionSolution
{
    // Both strategies decide the receptor from the parity of the crossing counts.
    private const int ParityDivisor = 2;

    // LC 858 receptor numbering: (p, 0) = 0, (p, q) = 1, (0, q) = 2 - "top-left".
    private const int BottomRightReceptor = 0;
    private const int TopRightReceptor = 1;
    private const int TopLeftReceptor = 2;

    // Brute force: advance the unfolded-grid crossing count k one room at a time
    // until the ray lands exactly on a horizontal grid line. BCL-only internals.
    public static int ReceptorBySimulatedUnfolding(int p, int q)
    {
        var k = 1;

        while ((long)k * q % p != 0)
        {
            k++;
        }

        var m = (long)k * q / p;
        return Receptor(ParityOf(k), ParityOf(m));
    }

    // The same k and m, reached in one Euclidean reduction instead of p steps.
    public static int ReceptorByGcdReduction(int p, int q)
    {
        var divisor = Gcd(p, q);
        var crossings = p / divisor;
        var rooms = q / divisor;

        return Receptor(ParityOf(crossings), ParityOf(rooms));
    }

    // The shared parity reading, so the two strategies differ only in how they
    // arrive at the crossing counts rather than in how they interpret them.
    private static int Receptor(Parity crossingParity, Parity roomParity)
    {
        if (crossingParity == Parity.Odd && roomParity == Parity.Even)
        {
            return BottomRightReceptor;
        }

        return crossingParity == Parity.Odd ? TopRightReceptor : TopLeftReceptor;
    }

    // The parity of a crossing count is the whole of what the receptor lookup reads
    // off it, named so a call site says which side of the reading the count lands on.
    private static Parity ParityOf(long value) =>
        IsEven(value) ? Parity.Even : Parity.Odd;

    // A crossing count the parity divisor divides exactly - the whole of the reading
    // above.
    private static bool IsEven(long value) => value % ParityDivisor == 0;

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    // The two parity readings Receptor branches on - each is a state of one crossing
    // count, named so the call site reads `Parity.Odd` rather than `true`.
    private enum Parity
    {
        // An even number of rooms crossed.
        Even,

        // An odd number of rooms crossed.
        Odd,
    }
}
