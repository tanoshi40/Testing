using System.Data;
using System.Text;
using mocking;
using MySqlConnector;

namespace database_testing;

/*
 create table person
(
    Id         char(36)     not null
        primary key,
    FirstName  varchar(255) not null,
    LastName   varchar(255) not null,
    Street     varchar(255) not null,
    City       varchar(255) not null,
    State      varchar(255) not null,
    PostalCode varchar(255) not null
);
*/

public static class DatabaseHandling
{
    private const string DbConnectionString = "server=127.0.0.1;uid=root;database=testing;AllowLoadLocalInfile=True";

    private const string InsertPersonSql = """
                                           INSERT INTO person (Id, FirstName, LastName, Street, City, State, PostalCode)
                                           VALUES (@Id, @FirstName, @LastName, @Street, @City, @State, @PostalCode)
                                           """;

    public static MySqlConnection GetConnection() => new(DbConnectionString);

    public static async Task Truncate(MySqlConnection connection)
    {
        const string sql = "truncate table person";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task InsertPersonAsync(Person person, MySqlConnection connection,
        MySqlTransaction? transaction = null)
    {
        await using var command = new MySqlCommand(InsertPersonSql, connection, transaction);
        command.Parameters.AddWithValue("@Id", person.Id.ToString());
        command.Parameters.AddWithValue("@FirstName", person.FirstName);
        command.Parameters.AddWithValue("@LastName", person.LastName);
        command.Parameters.AddWithValue("@Street", person.Street);
        command.Parameters.AddWithValue("@City", person.City);
        command.Parameters.AddWithValue("@State", person.State);
        command.Parameters.AddWithValue("@PostalCode", person.PostalCode);

        await command.ExecuteNonQueryAsync();
    }

    public static async Task InsertPersonsAsyncPrimitive(IEnumerable<Person> persons, MySqlConnection connection)
    {
        foreach (var person in persons)
        {
            await InsertPersonAsync(person, connection);
        }
    }

    public static async Task InsertPersonsAsyncTransaction(IEnumerable<Person> persons, MySqlConnection connection)
    {
        await using var transaction = await connection.BeginTransactionAsync();
        foreach (var person in persons)
        {
            await InsertPersonAsync(person, connection, transaction);
        }

        await transaction.CommitAsync();
    }

    public static async Task InsertPersonsAsyncBulkStringBuilder(IEnumerable<Person> persons,
        MySqlConnection connection, int batchSize = 5_000)
    {
        const string initialSql =
            "INSERT INTO Person (Id, FirstName, LastName, Street, City, State, PostalCode) VALUES ";
        var sql = new StringBuilder(initialSql);

        var parameters = new List<MySqlParameter>();
        var values = new List<string>();

        var count = 0;
        foreach (var person in persons)
        {
            var personValues =
                $"(@Id{count}, @FirstName{count}, @LastName{count}, @Street{count}, @City{count}, @State{count}, @PostalCode{count})";
            values.Add(personValues);

            parameters.AddRange(new[]
            {
                new MySqlParameter($"@Id{count}", person.Id.ToString()),
                new MySqlParameter($"@FirstName{count}", person.FirstName),
                new MySqlParameter($"@LastName{count}", person.LastName),
                new MySqlParameter($"@Street{count}", person.Street),
                new MySqlParameter($"@City{count}", person.City),
                new MySqlParameter($"@State{count}", person.State),
                new MySqlParameter($"@PostalCode{count}", person.PostalCode),
            });
            count++;

            if (count < batchSize)
            {
                continue;
            }

            await SendBulkInsertCandles(connection, sql, values, parameters);
            parameters.Clear();
            values.Clear();
            sql.Clear();
            sql.Append(initialSql);
            count = 0;
        }

        if (count > 0)
        {
            await SendBulkInsertCandles(connection, sql, values, parameters);
        }
    }

    private static async Task SendBulkInsertCandles(MySqlConnection connection, StringBuilder sql,
        IEnumerable<string> values, List<MySqlParameter> parameters)
    {
        sql.Append(string.Join(",", values));
        sql.Append(';');

        await using var command = new MySqlCommand(sql.ToString(), connection);
        command.Parameters.AddRange(parameters.ToArray());

        await command.ExecuteNonQueryAsync();
    }

    public static async Task InsertPersonAsyncBulkDataTable(IEnumerable<Person> persons, MySqlConnection connection)
    {
        const string srcTable = "person";

        var adapterSelect = new MySqlDataAdapter($"SELECT * FROM {srcTable} LIMIT 0", connection);

        var ds = new DataSet();
        adapterSelect.Fill(ds, srcTable);

        MySqlTransaction transaction = await connection.BeginTransactionAsync();

        var dataAdapter = new MySqlDataAdapter();
        dataAdapter.InsertCommand = new MySqlCommand(InsertPersonSql, connection);

        dataAdapter.InsertCommand.Transaction = transaction;

        dataAdapter.InsertCommand!.Parameters.Add(new MySqlParameter("@Id", MySqlDbType.VarChar, 36, "Id"));
        dataAdapter.InsertCommand.Parameters.Add(
            new MySqlParameter("@FirstName", MySqlDbType.VarChar, 255, "FirstName"));
        dataAdapter.InsertCommand.Parameters.Add(new MySqlParameter("@LastName", MySqlDbType.VarChar, 255, "LastName"));
        dataAdapter.InsertCommand.Parameters.Add(new MySqlParameter("@Street", MySqlDbType.VarChar, 255, "Street"));
        dataAdapter.InsertCommand.Parameters.Add(new MySqlParameter("@City", MySqlDbType.VarChar, 255, "City"));
        dataAdapter.InsertCommand.Parameters.Add(new MySqlParameter("@State", MySqlDbType.VarChar, 255, "State"));
        dataAdapter.InsertCommand.Parameters.Add(new MySqlParameter("@PostalCode", MySqlDbType.VarChar, 255,
            "PostalCode"));

        await dataAdapter.InsertCommand.PrepareAsync();

        dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

        foreach (var person in persons)
        {
            var dataTable = ds.Tables[srcTable]!;
            var row = dataTable.NewRow();

            row["Id"] = person.Id.ToString();
            row["FirstName"] = person.FirstName;
            row["LastName"] = person.LastName;
            row["Street"] = person.Street;
            row["City"] = person.City;
            row["State"] = person.State;
            row["PostalCode"] = person.PostalCode;
            dataTable.Rows.Add(row);
        }

        dataAdapter.UpdateBatchSize = 5_000;
        dataAdapter.Update(ds, srcTable);
        await transaction.CommitAsync();
    }

    public static async Task InsertPersonAsyncBulkCopy(IEnumerable<Person> persons, MySqlConnection connection)
    {
        // Create a DataTable to hold the data
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("FirstName");
        dataTable.Columns.Add("LastName");
        dataTable.Columns.Add("Street");
        dataTable.Columns.Add("City");
        dataTable.Columns.Add("State");
        dataTable.Columns.Add("PostalCode");

        // Add the person data to the DataTable
        foreach (var person in persons)
        {
            dataTable.Rows.Add(
                person.Id.ToString(),
                person.FirstName,
                person.LastName,
                person.Street,
                person.City,
                person.State,
                person.PostalCode
            );
        }

        // Perform the bulk insert
        var bulkCopy = new MySqlBulkCopy(connection)
        {
            DestinationTableName = "person"
        };

        await bulkCopy.WriteToServerAsync(dataTable);
    }
}