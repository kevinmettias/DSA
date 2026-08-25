namespace DSAExperimentation.Algorithms.Searching;

// Mutable by design, not a data-bag: BinarySearch.ProbeMidpoint takes this by ref and
// narrows Low/High in place as it eliminates one half of the range each step - the
// same in-place-mutation justification HashMapEntry uses for its own mutable fields.
internal struct SearchRange(int low, int high)
{
    public int Low = low;
    public int High = high;
}
