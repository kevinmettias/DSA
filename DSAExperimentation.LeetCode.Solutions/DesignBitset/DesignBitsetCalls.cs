namespace DSAExperimentation.LeetCode.Solutions.DesignBitset;

// LeetCode's toString call on the bitset, which IBitset answers as ToBitString, since every object already has
// a ToString of its own (ARCHITECTURE 17.13).
internal static class DesignBitsetCalls
{
    public static string ToString(DesignBitsetSolution.IBitset bitset) => bitset.ToBitString();
}
