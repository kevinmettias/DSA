namespace DSAExperimentation.LeetCode.FlowerPlantingWithNoAdjacent;

// LeetCode numbers LC 1042's gardens 1..n while the answer array is 0..n-1, so this
// is the offset between the two.
//
// Owned here rather than in FlowerPlantingWithNoAdjacentSolution because GardenNetwork
// and the benchmark both walk the gardens from it: it is the input's numbering, not
// anything either strategy decides.
internal static class GardenNumbering
{
    public const int FirstGarden = 1;
}
