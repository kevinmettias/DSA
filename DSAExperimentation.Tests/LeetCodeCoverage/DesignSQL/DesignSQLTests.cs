using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignSQL;

// LeetCode 2408. Design SQL: each table is this repo's own HashMap<int,string[]>
// keyed by an ever-increasing row id (deleteRow never reuses an id, matching the
// problem's own contract), with the table-name -> table lookup itself a second
// HashMap<string,Table> - the same two-level HashMap-of-HashMap composition
// DesignMovieRentalSystemTests' HashMap<int,BinarySearchTree<...>> already rehearses.
// insertRow/deleteRow/selectCell are then just Set/TryRemove/TryGetValue composed
// over those two maps; no new primitive needed.
public sealed partial class DesignSQLTests
{
    [Fact]
    public void Sql_InsertSelectDeleteThenSelectAgain_ReflectsEachMutation()
    {
        var sql = new SQL(["one", "two"], [2, 3]);

        sql.InsertRow("two", ["first", "second", "third"]);
        Assert.Equal("third", sql.SelectCell("two", 1, 3));

        sql.InsertRow("two", ["fourth", "fifth", "sixth"]);
        sql.DeleteRow("two", 1);

        Assert.Equal("fifth", sql.SelectCell("two", 2, 2));
    }

    [Fact]
    public void InsertRow_AssignsSequentialIdsAndNeverReusesADeletedOne()
    {
        var sql = new SQL(["t"], [1]);

        sql.InsertRow("t", ["a"]);
        sql.InsertRow("t", ["b"]);
        sql.DeleteRow("t", 1);
        sql.InsertRow("t", ["c"]);

        Assert.Equal("b", sql.SelectCell("t", 2, 1));
        Assert.Equal("c", sql.SelectCell("t", 3, 1));
    }

    [Fact]
    public void SelectCell_SeparateTables_DoNotShareRows()
    {
        var sql = new SQL(["one", "two"], [1, 1]);

        sql.InsertRow("one", ["alpha"]);
        sql.InsertRow("two", ["beta"]);

        Assert.Equal("alpha", sql.SelectCell("one", 1, 1));
        Assert.Equal("beta", sql.SelectCell("two", 1, 1));
    }

    private sealed class SQL
    {
        private readonly HashMap<string, Table> _tables = new();

        public SQL(string[] names, int[] columns)
        {
            for (var i = 0; i < names.Length; i++)
            {
                _tables.Set(names[i], new Table());
            }
        }

        public void InsertRow(string name, string[] row)
        {
            _tables.TryGetValue(name, out var table);
            table!.Insert(row);
        }

        public void DeleteRow(string name, int rowId)
        {
            _tables.TryGetValue(name, out var table);
            table!.Delete(rowId);
        }

        public string SelectCell(string name, int rowId, int columnId)
        {
            _tables.TryGetValue(name, out var table);
            return table!.Select(rowId, columnId);
        }

        private sealed class Table
        {
            private readonly HashMap<int, string[]> _rows = new();
            private int _nextRowId = 1;

            public void Insert(string[] row) => _rows.Set(_nextRowId++, row);

            public void Delete(int rowId) => _rows.TryRemove(rowId);

            public string Select(int rowId, int columnId)
            {
                _rows.TryGetValue(rowId, out var row);
                return row[columnId - 1];
            }
        }
    }
}
