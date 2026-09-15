namespace DSAExperimentation.LeetCode.ImplementRand10UsingRand7;

// The decision the two strategies are handed: where a uniform 1..7 integer comes
// from. It stays something the caller supplies rather than something a strategy
// owns, so the harness keeps control of seeding and of the call count.
internal interface IRand7
{
    // One uniform integer in 1..7. A caller may draw as many times as it needs to;
    // the source owns whatever state that takes.
    int Draw();
}
