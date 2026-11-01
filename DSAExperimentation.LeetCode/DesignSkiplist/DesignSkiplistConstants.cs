namespace DSAExperimentation.LeetCode.DesignSkiplist;

// The values LC 1206 fixes, declared once so DesignSkiplistSolution and its benchmark read the same
// copy instead of each restating it.
internal static class DesignSkiplistConstants
{
    // LeetCode 1206 constrains num to [0, 20000], which is what makes a flat
    // frequency array over the whole value domain affordable here.
    internal const int MaxValue = 20_000;
}
