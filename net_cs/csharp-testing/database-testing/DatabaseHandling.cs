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
    private const string DbConnectionString = "server=127.0.0.1;uid=root;database=testing";

    public static async Task InsertPersonAsync(Person person, MySqlConnection connection)
    {
        // SQL INSERT statement.
        const string sql = """
                            INSERT INTO Person (Id, FirstName, LastName, Street, City, State, PostalCode)
                            VALUES (@Id, @FirstName, @LastName, @Street, @City, @State, @PostalCode)
                           """;
        using var command = new MySqlCommand(sql, connection);

        // Setting the parameters.
        command.Parameters.AddWithValue("@Id", person.Id.ToString());
        command.Parameters.AddWithValue("@FirstName", person.FirstName);
        command.Parameters.AddWithValue("@LastName", person.LastName);
        command.Parameters.AddWithValue("@Street", person.Street);
        command.Parameters.AddWithValue("@City", person.City);
        command.Parameters.AddWithValue("@State", person.State);
        command.Parameters.AddWithValue("@PostalCode", person.PostalCode);

        // Execute the INSERT statement.
        await command.ExecuteNonQueryAsync();
    }
}