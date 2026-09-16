using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.FlowerPlantingWithNoAdjacent;

// LeetCode 1042. Flower Planting With No Adjacent: give each of gardenCount gardens
// one of four flower types so no two gardens joined by a path share a type.
//
// Every garden has at most 3 paths (LeetCode's own constraint), so one of the four
// types is always free: walking the gardens in order and taking the first
// available type always succeeds, and no backtracking is ever needed. Both
// strategies do exactly that and therefore return the same array; they differ only
// in how a garden's already-planted neighbors are found.
internal static class FlowerPlantingWithNoAdjacentSolution
{
    // Flower types are numbered 1..4, the same 1-based convention LeetCode uses
    // for the gardens themselves.
    private const int FirstFlowerType = 1;
    private const int FlowerTypeCount = 4;

    // A garden whose flower has not been chosen yet.
    private const int Unplanted = 0;

    // The textbook answer: no adjacency structure at all - rescan the whole raw
    // paths array for every garden to find its neighbors, and track the types they
    // already took in an int bitmask. O(gardens * paths), deliberately written with
    // nothing but BCL arrays; it is the arm the composed strategy below has to
    // justify itself against.
    public static int[] GardenNoAdjByRawPathRescan(int gardenCount, int[][] paths)
    {
        var flowers = new int[gardenCount];

        for (var garden = GardenNumbering.FirstGarden; garden <= gardenCount; garden++)
        {
            var usedMask = ComputeUsedFlowerMask(garden, paths, flowers);
            flowers[garden - GardenNumbering.FirstGarden] = FirstAvailableFlower(usedMask);
        }

        return flowers;
    }

    private static int ComputeUsedFlowerMask(int garden, int[][] paths, int[] flowers)
    {
        var used = 0;

        foreach (var path in paths)
        {
            if (TryGetNeighbor(garden, path, out var neighbor) &&
                flowers[neighbor - GardenNumbering.FirstGarden] != Unplanted)
            {
                used |= 1 << flowers[neighbor - GardenNumbering.FirstGarden];
            }
        }

        return used;
    }

    private static bool TryGetNeighbor(int garden, int[] path, out int neighbor)
    {
        if (path[0] == garden)
        {
            neighbor = path[1];
            return true;
        }

        if (path[1] == garden)
        {
            neighbor = path[0];
            return true;
        }

        neighbor = default;
        return false;
    }

    private static int FirstAvailableFlower(int usedMask)
    {
        for (var candidate = FirstFlowerType; candidate <= FlowerTypeCount; candidate++)
        {
            if ((usedMask & (1 << candidate)) == 0)
            {
                return candidate;
            }
        }

        return Unplanted;
    }

    // This repo's own primitives: adjacency built once as ListChildren over a
    // GardenTopology, so each garden reads only its own neighbors rather than the
    // whole paths array, and Set<int> tracks the types they took instead of a
    // hand-rolled bool[4] or bitmask.
    public static int[] GardenNoAdjByAdjacencyList(int gardenCount, int[][] paths)
    {
        var network = GardenNetwork.Build(gardenCount, paths);
        return GardenNoAdjByAdjacencyList(network);
    }

    public static int[] GardenNoAdjByAdjacencyList(GardenNetwork network)
    {
        var flowers = new int[network.Gardens.Count];

        foreach (var garden in network.Gardens)
        {
            var usedFlowers = ComputeUsedFlowerSet(garden, flowers);
            flowers[garden.Id - GardenNumbering.FirstGarden] = FirstAvailableFlower(usedFlowers);
        }

        return flowers;
    }

    private static Set<int> ComputeUsedFlowerSet(GardenNode garden, int[] flowers)
    {
        var usedFlowers = new Set<int>();
        var neighbors = GardenTopology.GetChildren(garden);

        for (var i = 0; i < neighbors.Count; i++)
        {
            var neighborFlower = flowers[neighbors.Get(i).Id - GardenNumbering.FirstGarden];

            if (neighborFlower != Unplanted)
            {
                usedFlowers.TryAdd(neighborFlower);
            }
        }

        return usedFlowers;
    }

    private static int FirstAvailableFlower(Set<int> usedFlowers)
    {
        for (var candidate = FirstFlowerType; candidate <= FlowerTypeCount; candidate++)
        {
            if (!usedFlowers.Has(candidate))
            {
                return candidate;
            }
        }

        return Unplanted;
    }
}
