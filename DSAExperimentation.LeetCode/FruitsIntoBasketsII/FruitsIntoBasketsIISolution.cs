namespace DSAExperimentation.LeetCode.FruitsIntoBasketsII;

// LeetCode 3477. Fruits Into Baskets II: place each fruit type, left to right,
// into the leftmost still-empty basket whose capacity can hold it; count how
// many fruit types end up unplaced. n <= 100, so the direct O(n^2) rescan is
// this problem's own intended solution - no repo primitive earns its keep at
// this size. See LC 3479 (Fruits Into Baskets III) for the identical rule at
// n <= 1e5, where this repo's SegmentTree<int, MaxOperation<int>> does.
internal static class FruitsIntoBasketsIISolution
{
    public static int CountUnplacedByBruteForce(int[] fruits, int[] baskets)
    {
        var used = new bool[baskets.Length];
        var unplaced = 0;

        foreach (var quantity in fruits)
        {
            var placed = false;

            for (var j = 0; j < baskets.Length; j++)
            {
                if (!used[j] && baskets[j] >= quantity)
                {
                    used[j] = true;
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                unplaced++;
            }
        }

        return unplaced;
    }
}
