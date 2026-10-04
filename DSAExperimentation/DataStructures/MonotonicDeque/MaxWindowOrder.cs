namespace DSAExperimentation.DataStructures.MonotonicDeque;

// The window's maximum: a resident is dominated by an arriving key at least as large.
internal readonly struct MaxWindowOrder<Key> : IWindowOrder<Key>
    where Key : IComparable<Key>
{
    public static bool IsDominatedBy(Key resident, Key arriving) => arriving.CompareTo(resident) >= 0;
}
