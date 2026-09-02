namespace DSAExperimentation.LeetCode.MinimumCostToBuyApplesII;

// The graph LC 3928 poses its query over, built once from LeetCode's raw
// (n, prices, roads) shape: each undirected road contributes one entry to both
// endpoints' ForwardEdges (weight = cost) and both endpoints' ReturnEdges
// (weight = cost * tax). Both solution strategies below share this same prepared
// network; only how each one searches it differs.
internal sealed class AppleNetwork
{
    private AppleNetwork(AppleNode[] nodes, int[] prices)
    {
        Nodes = nodes;
        Prices = prices;
    }

    public AppleNode[] Nodes { get; }

    public int[] Prices { get; }

    public static AppleNetwork Build(int n, int[] prices, int[][] roads)
    {
        var nodes = new AppleNode[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new AppleNode(i);
        }

        foreach (var road in roads)
        {
            AddRoad(nodes, road);
        }

        return new AppleNetwork(nodes, prices);
    }

    private static void AddRoad(AppleNode[] nodes, int[] road)
    {
        var u = nodes[road[0]];
        var v = nodes[road[1]];
        var cost = (long)road[2];
        var tax = (long)road[3];

        u.ForwardEdges.Add((cost, v));
        v.ForwardEdges.Add((cost, u));
        u.ReturnEdges.Add((cost * tax, v));
        v.ReturnEdges.Add((cost * tax, u));
    }
}
