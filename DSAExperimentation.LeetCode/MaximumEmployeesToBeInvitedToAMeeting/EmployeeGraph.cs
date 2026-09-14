namespace DSAExperimentation.LeetCode.MaximumEmployeesToBeInvitedToAMeeting;

// LC 2127's input reshaped once into the node form the graph strategy walks: one
// EmployeeNode per index, each holding exactly the one successor favorite[i]
// names.
//
// Bespoke to this one problem, which is why it lives beside the solution rather
// than in Domain (ARCHITECTURE.md #17.3): nothing here fixes a modulus or a vertex
// set another problem could share - it is simply this problem's own input shape,
// hoisted out of the measured strategy so a benchmark can build it once in
// [GlobalSetup] (#17.4). Being a named type rather than a bare List<EmployeeNode>
// is what keeps the hoisted overload unambiguous against LeetCode's own int[]
// shape, the same role MeetingSchedule plays for LC 2092.
internal readonly record struct EmployeeGraph(List<EmployeeNode> Nodes)
{
    public static EmployeeGraph Build(int[] favorite)
    {
        var nodes = Enumerable.Range(0, favorite.Length).Select(id => new EmployeeNode(id)).ToList();

        for (var i = 0; i < favorite.Length; i++)
        {
            nodes[i].Successors.Add(nodes[favorite[i]]);
        }

        return new EmployeeGraph(nodes);
    }
}
