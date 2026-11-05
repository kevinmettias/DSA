using DSAExperimentation.LeetCode.Solutions.DesignSQL;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DesignSQL;

// One table of two columns.
public sealed partial class DesignSQLCallsTests
{
    private const string Table = "two";
    private const string First = "first";
    private const string Second = "second";

    // The exported row: its id, then its values, comma-separated.
    private const string ExportedRow = "1,first,second";

    private static readonly string[] Row = [First, Second];

    [Fact]
    public void Ins_RowOfTheTablesWidth_IsInserted() => Assert.True(DesignSQLCalls.Ins(Database(), Table, Row));

    [Fact]
    public void Sel_InsertedCell_IsItsValue()
    {
        var database = Database();
        DesignSQLCalls.Ins(database, Table, Row);

        Assert.Equal(Second, DesignSQLCalls.Sel(database, Table, 1, 2));
    }

    [Fact]
    public void Exp_TableWithOneRow_ExportsIt()
    {
        var database = Database();
        DesignSQLCalls.Ins(database, Table, Row);

        Assert.Equal([ExportedRow], DesignSQLCalls.Exp(database, Table));
    }

    [Fact]
    public void Rmv_InsertedRow_LeavesNothingToExport()
    {
        var database = Database();
        DesignSQLCalls.Ins(database, Table, Row);

        DesignSQLCalls.Rmv(database, Table, 1);

        Assert.Empty(DesignSQLCalls.Exp(database, Table));
    }

    private static DesignSQLSolution.ISqlStrategy Database() => DesignSQLSolution.CreateByListScan([Table], [2]);
}
