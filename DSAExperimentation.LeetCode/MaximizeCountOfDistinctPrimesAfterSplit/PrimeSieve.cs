using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.MaximizeCountOfDistinctPrimesAfterSplit;

// The sieve of Eratosthenes over a value bound: slot i is true when i is composite, so "is slot i
// clear" is the primality test, for every value up to the bound at once.
//
// Kept local rather than taken from the core's PrimeSieve because this problem's baseline arm
// reaches it as well as its composed arm (ARCHITECTURE section 17.5): the two arms share one
// sieve, so neither can move to a library primitive without the other.
internal static class PrimeSieve
{
    // The whole tracker for [0, bound], one flag per value - what CountPrimes names a
    // composite tracker and CountValidPathsInATree names its sieve with a dynamic
    // array, so the verb-led name is this corpus's word for the same returned value.
    // Slot i is true when i is composite. Values below the smallest prime count as
    // composite, which is what lets a caller answer primality for 0 and 1 with the
    // same single "is the slot set" test it uses everywhere else.
    //
    // The multiple's own index is taken as a long before it is squared: a bound near
    // int.MaxValue squares past what an int holds, and the overflow would break the
    // loop's exit condition rather than the sieve itself.
    public static DynamicArray<bool> BuildCompositeTracker(int bound)
    {
        var isComposite = new DynamicArray<bool>();

        for (var i = 0; i <= bound; i++)
        {
            isComposite.Add(i < 2);
        }

        for (var i = 2; (long)i * i <= bound; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= bound; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        return isComposite;
    }
}
