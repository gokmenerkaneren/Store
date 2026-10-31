using System;
using Npgsql;
class Program {
    static void Main() {
        var connStr = ""Server=localhost;Port=5432;Database=StoreLocationDb;Username=postgres;Password=sa435558;"";
        using var conn = new NpgsqlConnection(connStr);
        conn.Open();
        using var cmd = new NpgsqlCommand(""SELECT COUNT(*) FROM \""Cities\"""", conn);
        Console.WriteLine(""Cities: "" + cmd.ExecuteScalar());
        using var cmd2 = new NpgsqlCommand(""SELECT COUNT(*) FROM \""Districts\"""", conn);
        Console.WriteLine(""Districts: "" + cmd2.ExecuteScalar());
    }
}
